using MediatR;
using PeladaPay.Application.DTOs;
using PeladaPay.Domain.Entities;
using PeladaPay.Domain.Interfaces;

namespace PeladaPay.Application.Features.Auth.Commands;

public sealed record CompleteOnboardingComplianceCommand(
    Guid SessionId,
    string Cpf,
    DateOnly BirthDate,
    string Address) : IRequest<OnboardingStepResponseDto>;

public sealed class CompleteOnboardingComplianceCommandHandler(
    IRepository<OnboardingSession> onboardingRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CompleteOnboardingComplianceCommand, OnboardingStepResponseDto>
{
    public async Task<OnboardingStepResponseDto> Handle(CompleteOnboardingComplianceCommand request, CancellationToken cancellationToken)
    {
        var session = await onboardingRepository.GetByIdAsync(request.SessionId, cancellationToken)
            ?? throw new InvalidOperationException("Sessão de onboarding não encontrada.");

        if (session.CompletedAtUtc is not null)
        {
            throw new InvalidOperationException("Essa sessão de onboarding já foi finalizada.");
        }

        session.Cpf = request.Cpf.Trim();
        session.BirthDate = request.BirthDate;
        session.Address = request.Address.Trim();
        session.CurrentStep = Math.Max(session.CurrentStep, 2);

        onboardingRepository.Update(session);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new OnboardingStepResponseDto(session.Id, 2, 4, "pelada");
    }
}
