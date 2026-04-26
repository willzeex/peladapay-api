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
    IRepository<OnboardingGroupDraft> onboardingGroupDraftRepository,
    IRepository<FinancialAccount> accountRepository,
    IRepository<Group> groupRepository,
    IUnitOfWork unitOfWork,
    IJwtTokenGenerator jwtTokenGenerator) : IRequestHandler<CompleteOnboardingFinancialCommand, OnboardingResponseDto>
{
    public async Task<OnboardingResponseDto> Handle(CompleteOnboardingFinancialCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.SessionId.ToString())
            ?? throw new InvalidOperationException("Sessão de onboarding não encontrada.");

        if (user.OnboardingCompletedAtUtc is not null)
        {
            throw new InvalidOperationException("Essa sessão de onboarding já foi finalizada.");
        }

        var draft = (await onboardingGroupDraftRepository.GetAsync(x => x.UserId == user.Id, cancellationToken))
            .SingleOrDefault();

        if (user.OnboardingCurrentStep < 3 || string.IsNullOrWhiteSpace(user.Cpf) || user.BirthDate is null
            || string.IsNullOrWhiteSpace(user.Address) || draft is null || string.IsNullOrWhiteSpace(draft.Name)
            || draft.Frequency is null || !user.PlanId.HasValue)
        {
            throw new InvalidOperationException("Complete as etapas anteriores antes de finalizar o onboarding.");
        }

        var account = new FinancialAccount
        {
            Balance = 0,
            PixKey = user.Cpf,
            ExternalSubaccountId = $"subacc_{Guid.NewGuid():N}",
            MonthlyFee = request.MonthlyFee,
            SingleMatchFee = request.SingleMatchFee,
            DueDay = request.DueDay,
            IsExpenseManagementOnly = request.IsExpenseManagementOnly
        };

        await accountRepository.AddAsync(account, cancellationToken);

        var group = new Group
        {
            Name = draft!.Name,
            MatchDate = DateTime.UtcNow,
            Frequency = draft.Frequency!.Value,
            Venue = draft.Venue,
            CrestUrl = draft.CrestUrl,
            FinancialAccountId = account.Id,
            OrganizerId = user.Id
        };

        await groupRepository.AddAsync(group, cancellationToken);

        user.OnboardingCurrentStep = 4;
        user.OnboardingCompletedAtUtc = DateTime.UtcNow;

        var updateResult = await userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            var errors = string.Join("; ", updateResult.Errors.Select(x => x.Description));
            throw new InvalidOperationException(errors);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var token = jwtTokenGenerator.Generate(user);

        return new OnboardingResponseDto(
            new AuthResponseDto(user.Id, user.Email!, token),
            new GroupDto(group.Id, group.Name, group.MatchDate, group.FinancialAccountId, account.ExternalSubaccountId));
    }
}
