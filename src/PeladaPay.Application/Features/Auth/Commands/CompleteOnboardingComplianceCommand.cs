using MediatR;
using Microsoft.AspNetCore.Identity;
using PeladaPay.Application.DTOs;
using PeladaPay.Domain.Entities;

namespace PeladaPay.Application.Features.Auth.Commands;

public sealed record CompleteOnboardingComplianceCommand(
    Guid SessionId,
    string Cpf,
    DateOnly BirthDate,
    string Address) : IRequest<OnboardingStepResponseDto>;

public sealed class CompleteOnboardingComplianceCommandHandler(
    UserManager<ApplicationUser> userManager) : IRequestHandler<CompleteOnboardingComplianceCommand, OnboardingStepResponseDto>
{
    public async Task<OnboardingStepResponseDto> Handle(CompleteOnboardingComplianceCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.SessionId.ToString())
            ?? throw new InvalidOperationException("Sessão de onboarding não encontrada.");

        if (user.OnboardingCompletedAtUtc is not null)
        {
            throw new InvalidOperationException("Essa sessão de onboarding já foi finalizada.");
        }

        user.Cpf = request.Cpf.Trim();
        user.BirthDate = request.BirthDate;
        user.Address = request.Address.Trim();
        user.OnboardingCurrentStep = Math.Max(user.OnboardingCurrentStep, 2);

        var updateResult = await userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            var errors = string.Join("; ", updateResult.Errors.Select(x => x.Description));
            throw new InvalidOperationException(errors);
        }

        return new OnboardingStepResponseDto(request.SessionId, 2, 4, "pelada");
    }
}
