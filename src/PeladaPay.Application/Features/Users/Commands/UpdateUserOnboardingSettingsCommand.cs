using MediatR;
using Microsoft.AspNetCore.Identity;
using PeladaPay.Application.Interfaces;
using PeladaPay.Domain.Entities;

namespace PeladaPay.Application.Features.Users.Commands;

public sealed record UpdateUserOnboardingSettingsCommand(
    string? OnboardingGroupName,
    string? OnboardingFrequency,
    string? OnboardingVenue,
    string? OnboardingCrestUrl) : IRequest<UpdateUserOnboardingSettingsResponse>;

public sealed record UpdateUserOnboardingSettingsResponse(
    string? OnboardingGroupName,
    string? OnboardingFrequency,
    string? OnboardingVenue,
    string? OnboardingCrestUrl);

public sealed class UpdateUserOnboardingSettingsCommandHandler(
    UserManager<ApplicationUser> userManager,
    ICurrentUserService currentUserService) : IRequestHandler<UpdateUserOnboardingSettingsCommand, UpdateUserOnboardingSettingsResponse>
{
    public async Task<UpdateUserOnboardingSettingsResponse> Handle(UpdateUserOnboardingSettingsCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException();
        var user = await userManager.FindByIdAsync(userId)
            ?? throw new InvalidOperationException("Usuário autenticado não encontrado.");

        var hasChanges = false;

        if (request.OnboardingGroupName is not null)
        {
            user.OnboardingGroupName = string.IsNullOrWhiteSpace(request.OnboardingGroupName)
                ? null
                : request.OnboardingGroupName.Trim();
            hasChanges = true;
        }

        if (request.OnboardingFrequency is not null)
        {
            user.OnboardingFrequency = string.IsNullOrWhiteSpace(request.OnboardingFrequency)
                ? null
                : request.OnboardingFrequency.Trim();
            hasChanges = true;
        }

        if (request.OnboardingVenue is not null)
        {
            user.OnboardingVenue = string.IsNullOrWhiteSpace(request.OnboardingVenue)
                ? null
                : request.OnboardingVenue.Trim();
            hasChanges = true;
        }

        if (request.OnboardingCrestUrl is not null)
        {
            user.OnboardingCrestUrl = string.IsNullOrWhiteSpace(request.OnboardingCrestUrl)
                ? null
                : request.OnboardingCrestUrl.Trim();
            hasChanges = true;
        }

        if (hasChanges)
        {
            var updateResult = await userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var errors = string.Join("; ", updateResult.Errors.Select(x => x.Description));
                throw new InvalidOperationException(errors);
            }
        }

        return new UpdateUserOnboardingSettingsResponse(
            user.OnboardingGroupName,
            user.OnboardingFrequency,
            user.OnboardingVenue,
            user.OnboardingCrestUrl);
    }
}
