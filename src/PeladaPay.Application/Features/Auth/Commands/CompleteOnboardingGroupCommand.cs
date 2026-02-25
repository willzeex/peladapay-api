using MediatR;
using Microsoft.AspNetCore.Identity;
using PeladaPay.Application.DTOs;
using PeladaPay.Domain.Entities;

namespace PeladaPay.Application.Features.Auth.Commands;

public sealed record CompleteOnboardingGroupCommand(
    Guid SessionId,
    string GroupName,
    string Frequency,
    string? Venue,
    string? CrestUrl) : IRequest<OnboardingStepResponseDto>;

public sealed class CompleteOnboardingGroupCommandHandler(
    UserManager<ApplicationUser> userManager) : IRequestHandler<CompleteOnboardingGroupCommand, OnboardingStepResponseDto>
{
    public async Task<OnboardingStepResponseDto> Handle(CompleteOnboardingGroupCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.SessionId.ToString())
            ?? throw new InvalidOperationException("Sessão de onboarding não encontrada.");

        if (user.OnboardingCompletedAtUtc is not null)
        {
            throw new InvalidOperationException("Essa sessão de onboarding já foi finalizada.");
        }

        if (user.OnboardingCurrentStep < 2)
        {
            throw new InvalidOperationException("Complete a etapa de compliance antes da pelada.");
        }

        user.OnboardingGroupName = request.GroupName.Trim();
        user.OnboardingFrequency = request.Frequency.Trim();
        user.OnboardingVenue = request.Venue?.Trim();
        user.OnboardingCrestUrl = request.CrestUrl?.Trim();
        user.OnboardingCurrentStep = Math.Max(user.OnboardingCurrentStep, 3);

        var updateResult = await userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            var errors = string.Join("; ", updateResult.Errors.Select(x => x.Description));
            throw new InvalidOperationException(errors);
        }

        return new OnboardingStepResponseDto(request.SessionId, 3, 4, "financeiro");
    }
}
