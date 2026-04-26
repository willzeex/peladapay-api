using MediatR;
using Microsoft.AspNetCore.Identity;
using PeladaPay.Application.Interfaces;
using PeladaPay.Domain.Entities;
using PeladaPay.Domain.Enums;
using PeladaPay.Domain.Exceptions;
using PeladaPay.Domain.Interfaces;

namespace PeladaPay.Application.Features.Users.Commands;

public sealed record UpdateUserOnboardingSettingsCommand(
    string? OnboardingGroupName,
    GroupFrequency? OnboardingFrequency,
    string? OnboardingVenue,
    string? OnboardingCrestUrl,
    Guid? PlanId) : IRequest<UpdateUserOnboardingSettingsResponse>;

public sealed record UpdateUserOnboardingSettingsResponse(
    string? OnboardingGroupName,
    GroupFrequency? OnboardingFrequency,
    string? OnboardingVenue,
    string? OnboardingCrestUrl,
    Guid? PlanId);

public sealed class UpdateUserOnboardingSettingsCommandHandler(
    UserManager<ApplicationUser> userManager,
    IRepository<OnboardingGroupDraft> onboardingGroupDraftRepository,
    IRepository<Plan> planRepository,
    ICurrentUserService currentUserService,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateUserOnboardingSettingsCommand, UpdateUserOnboardingSettingsResponse>
{
    public async Task<UpdateUserOnboardingSettingsResponse> Handle(UpdateUserOnboardingSettingsCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException();
        var user = await userManager.FindByIdAsync(userId)
            ?? throw new InvalidOperationException("Usuário autenticado não encontrado.");
        var draft = (await onboardingGroupDraftRepository.GetAsync(x => x.UserId == user.Id, cancellationToken))
            .SingleOrDefault();
        var draftAlreadyExists = draft is not null;

        var hasDraftChanges = false;
        var hasUserChanges = false;

        if (request.OnboardingGroupName is not null)
        {
            draft ??= new OnboardingGroupDraft { UserId = user.Id };
            draft.Name = string.IsNullOrWhiteSpace(request.OnboardingGroupName)
                ? null
                : request.OnboardingGroupName.Trim();
            hasDraftChanges = true;
        }

        if (request.OnboardingFrequency is not null)
        {
            draft ??= new OnboardingGroupDraft { UserId = user.Id };
            draft.Frequency = request.OnboardingFrequency;
            hasDraftChanges = true;
        }

        if (request.OnboardingVenue is not null)
        {
            draft ??= new OnboardingGroupDraft { UserId = user.Id };
            draft.Venue = string.IsNullOrWhiteSpace(request.OnboardingVenue)
                ? null
                : request.OnboardingVenue.Trim();
            hasDraftChanges = true;
        }

        if (request.OnboardingCrestUrl is not null)
        {
            draft ??= new OnboardingGroupDraft { UserId = user.Id };
            draft.CrestUrl = string.IsNullOrWhiteSpace(request.OnboardingCrestUrl)
                ? null
                : request.OnboardingCrestUrl.Trim();
            hasDraftChanges = true;
        }

        if (request.PlanId.HasValue)
        {
            var plan = await planRepository.GetByIdAsync(request.PlanId.Value, cancellationToken)
                ?? throw new NotFoundException("Plano informado não encontrado.");
            user.PlanId = plan.Id;
            hasUserChanges = true;
        }

        if (hasDraftChanges)
        {
            if (draftAlreadyExists)
            {
                onboardingGroupDraftRepository.Update(draft!);
            }
            else
            {
                await onboardingGroupDraftRepository.AddAsync(draft!, cancellationToken);
            }
        }

        if (hasUserChanges)
        {
            var updateResult = await userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var errors = string.Join("; ", updateResult.Errors.Select(x => x.Description));
                throw new InvalidOperationException(errors);
            }
        }

        if (hasDraftChanges)
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return new UpdateUserOnboardingSettingsResponse(
            draft?.Name,
            draft?.Frequency,
            draft?.Venue,
            draft?.CrestUrl,
            user.PlanId);
    }
}
