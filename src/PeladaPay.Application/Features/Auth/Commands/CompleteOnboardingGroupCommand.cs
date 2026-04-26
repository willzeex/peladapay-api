using MediatR;
using Microsoft.AspNetCore.Identity;
using PeladaPay.Application.DTOs;
using PeladaPay.Domain.Entities;
using PeladaPay.Domain.Enums;
using PeladaPay.Domain.Exceptions;
using PeladaPay.Domain.Interfaces;

namespace PeladaPay.Application.Features.Auth.Commands;

public sealed record CompleteOnboardingGroupCommand(
    Guid SessionId,
    string GroupName,
    GroupFrequency Frequency,
    string? Venue,
    string? CrestUrl,
    Guid PlanId) : IRequest<OnboardingStepResponseDto>;

public sealed class CompleteOnboardingGroupCommandHandler(
    UserManager<ApplicationUser> userManager,
    IRepository<Plan> planRepository,
    IRepository<OnboardingGroupDraft> onboardingGroupDraftRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CompleteOnboardingGroupCommand, OnboardingStepResponseDto>
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

        var draft = (await onboardingGroupDraftRepository.GetAsync(x => x.UserId == user.Id, cancellationToken))
            .SingleOrDefault();

        if (draft is null)
        {
            draft = new OnboardingGroupDraft
            {
                UserId = user.Id,
                Name = request.GroupName.Trim(),
                Frequency = request.Frequency,
                Venue = request.Venue?.Trim(),
                CrestUrl = request.CrestUrl?.Trim()
            };

            await onboardingGroupDraftRepository.AddAsync(draft, cancellationToken);
        }
        else
        {
            draft.Name = request.GroupName.Trim();
            draft.Frequency = request.Frequency;
            draft.Venue = request.Venue?.Trim();
            draft.CrestUrl = request.CrestUrl?.Trim();
            onboardingGroupDraftRepository.Update(draft);
        }

        var plan = await planRepository.GetByIdAsync(request.PlanId, cancellationToken)
            ?? throw new NotFoundException("Plano informado não encontrado.");

        user.PlanId = plan.Id;
        user.OnboardingCurrentStep = Math.Max(user.OnboardingCurrentStep, 3);

        var updateResult = await userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            var errors = string.Join("; ", updateResult.Errors.Select(x => x.Description));
            throw new InvalidOperationException(errors);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new OnboardingStepResponseDto(request.SessionId, 3, 4, "financeiro");
    }
}
