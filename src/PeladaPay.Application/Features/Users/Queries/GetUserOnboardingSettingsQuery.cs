using MediatR;
using Microsoft.AspNetCore.Identity;
using PeladaPay.Application.Interfaces;
using PeladaPay.Domain.Entities;
using PeladaPay.Domain.Enums;
using PeladaPay.Domain.Interfaces;

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
    IRepository<OnboardingGroupDraft> onboardingGroupDraftRepository,
    ICurrentUserService currentUserService) : IRequestHandler<GetUserOnboardingSettingsQuery, GetUserOnboardingSettingsResponse>
{
    public async Task<GetUserOnboardingSettingsResponse> Handle(GetUserOnboardingSettingsQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException();
        var user = await userManager.FindByIdAsync(userId)
            ?? throw new InvalidOperationException("Usuário autenticado não encontrado.");
        var draft = (await onboardingGroupDraftRepository.GetAsync(x => x.UserId == user.Id, cancellationToken))
            .SingleOrDefault();

        return new GetUserOnboardingSettingsResponse(
            draft?.Name ?? string.Empty,
            draft?.Frequency,
            draft?.Venue ?? string.Empty,
            draft?.CrestUrl ?? string.Empty,
            user.PlanId);
    }
}
