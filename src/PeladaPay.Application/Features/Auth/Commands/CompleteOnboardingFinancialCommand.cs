using MediatR;
using Microsoft.AspNetCore.Identity;
using PeladaPay.Application.DTOs;
using PeladaPay.Application.Interfaces;
using PeladaPay.Domain.Entities;
using PeladaPay.Domain.Interfaces;

namespace PeladaPay.Application.Features.Auth.Commands;

public sealed record CompleteOnboardingFinancialCommand(
    Guid SessionId,
    decimal MonthlyFee,
    decimal SingleMatchFee,
    int DueDay,
    bool IsExpenseManagementOnly) : IRequest<OnboardingResponseDto>;

public sealed class CompleteOnboardingFinancialCommandHandler(
    UserManager<ApplicationUser> userManager,
    IRepository<OnboardingSession> onboardingRepository,
    IRepository<FinancialAccount> accountRepository,
    IRepository<Group> groupRepository,
    IUnitOfWork unitOfWork,
    IJwtTokenGenerator jwtTokenGenerator) : IRequestHandler<CompleteOnboardingFinancialCommand, OnboardingResponseDto>
{
    public async Task<OnboardingResponseDto> Handle(CompleteOnboardingFinancialCommand request, CancellationToken cancellationToken)
    {
        var session = await onboardingRepository.GetByIdAsync(request.SessionId, cancellationToken)
            ?? throw new InvalidOperationException("Sessão de onboarding não encontrada.");

        if (session.CompletedAtUtc is not null)
        {
            throw new InvalidOperationException("Essa sessão de onboarding já foi finalizada.");
        }

        if (session.CurrentStep < 3 || string.IsNullOrWhiteSpace(session.Cpf) || session.BirthDate is null
            || string.IsNullOrWhiteSpace(session.Address) || string.IsNullOrWhiteSpace(session.GroupName)
            || string.IsNullOrWhiteSpace(session.Frequency))
        {
            throw new InvalidOperationException("Complete as etapas anteriores antes de finalizar o onboarding.");
        }

        var user = new ApplicationUser
        {
            UserName = session.Email,
            Email = session.Email,
            FullName = session.FullName,
            PhoneNumber = session.Whatsapp,
            Whatsapp = session.Whatsapp,
            Cpf = session.Cpf,
            BirthDate = session.BirthDate,
            Address = session.Address
        };

        var userCreationResult = await userManager.CreateAsync(user, session.Password);
        if (!userCreationResult.Succeeded)
        {
            var errors = string.Join("; ", userCreationResult.Errors.Select(x => x.Description));
            throw new InvalidOperationException(errors);
        }

        var account = new FinancialAccount
        {
            Balance = 0,
            PixKey = session.Cpf,
            ExternalSubaccountId = $"subacc_{Guid.NewGuid():N}",
            MonthlyFee = request.MonthlyFee,
            SingleMatchFee = request.SingleMatchFee,
            DueDay = request.DueDay,
            IsExpenseManagementOnly = request.IsExpenseManagementOnly
        };

        await accountRepository.AddAsync(account, cancellationToken);

        var group = new Group
        {
            Name = session.GroupName,
            MatchDate = DateTime.UtcNow,
            Frequency = session.Frequency,
            Venue = session.Venue,
            CrestUrl = session.CrestUrl,
            FinancialAccountId = account.Id,
            OrganizerId = user.Id
        };

        await groupRepository.AddAsync(group, cancellationToken);

        session.CurrentStep = 4;
        session.CompletedAtUtc = DateTime.UtcNow;
        onboardingRepository.Update(session);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var token = jwtTokenGenerator.Generate(user);

        return new OnboardingResponseDto(
            new AuthResponseDto(user.Id, user.Email!, token),
            new GroupDto(group.Id, group.Name, group.MatchDate, group.FinancialAccountId, account.ExternalSubaccountId));
    }
}
