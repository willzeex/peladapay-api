using MediatR;
using PeladaPay.Application.DTOs;
using PeladaPay.Domain.Entities;
using PeladaPay.Domain.Interfaces;

namespace PeladaPay.Application.Features.Auth.Commands;

public sealed record CompleteOnboardingGroupCommand(
    Guid SessionId,
    string GroupName,
    string Frequency,
    string? Venue,
    string? CrestUrl) : IRequest<OnboardingStepResponseDto>;

public sealed class CompleteOnboardingGroupCommandHandler(
    IRepository<OnboardingSession> onboardingRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CompleteOnboardingGroupCommand, OnboardingStepResponseDto>
{
    public async Task<OnboardingStepResponseDto> Handle(CompleteOnboardingGroupCommand request, CancellationToken cancellationToken)
    {
        var session = await onboardingRepository.GetByIdAsync(request.SessionId, cancellationToken)
            ?? throw new InvalidOperationException("Sessão de onboarding não encontrada.");

        if (session.CompletedAtUtc is not null)
        {
            throw new InvalidOperationException("Essa sessão de onboarding já foi finalizada.");
        }

        if (session.CurrentStep < 2)
        {
            throw new InvalidOperationException("Complete a etapa de compliance antes da pelada.");
        }

        session.GroupName = request.GroupName.Trim();
        session.Frequency = request.Frequency.Trim();
        session.Venue = request.Venue?.Trim();
        session.CrestUrl = request.CrestUrl?.Trim();
        session.CurrentStep = Math.Max(session.CurrentStep, 3);

        onboardingRepository.Update(session);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new OnboardingStepResponseDto(session.Id, 3, 4, "financeiro");
    }
}
