using MediatR;
using Microsoft.AspNetCore.Identity;
using PeladaPay.Application.Interfaces;
using PeladaPay.Domain.Entities;
using PeladaPay.Domain.Enums;

namespace PeladaPay.Application.Features.Users.Queries;

public sealed record GetUserOnboardingSettingsQuery : IRequest<GetUserOnboardingSettingsResponse>;

public sealed record GetUserOnboardingSettingsResponse(
    string OnboardingGroupName,
    GroupFrequency? OnboardingFrequency,
    string OnboardingVenue,
    string OnboardingCrestUrl,
    Guid? PlanId);

public sealed class GetUserOnboardingSettingsQueryHandler(
    UserManager<ApplicationUser> userManager,
    ICurrentUserService currentUserService) : IRequestHandler<GetUserOnboardingSettingsQuery, GetUserOnboardingSettingsResponse>
{
    public async Task<GetUserOnboardingSettingsResponse> Handle(GetUserOnboardingSettingsQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException();
        var user = await userManager.FindByIdAsync(userId)
            ?? throw new InvalidOperationException("Usuário autenticado não encontrado.");

        return new GetUserOnboardingSettingsResponse(
            user.OnboardingGroupName ?? string.Empty,
            user.OnboardingFrequency,
            user.OnboardingVenue ?? string.Empty,
            user.OnboardingCrestUrl ?? string.Empty,
            user.PlanId);
    }
}
