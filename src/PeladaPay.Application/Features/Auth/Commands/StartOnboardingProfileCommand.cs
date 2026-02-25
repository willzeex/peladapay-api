using MediatR;
using PeladaPay.Application.DTOs;
using PeladaPay.Domain.Entities;
using PeladaPay.Domain.Interfaces;

namespace PeladaPay.Application.Features.Auth.Commands;

public sealed record StartOnboardingProfileCommand(
    string FullName,
    string Email,
    string Whatsapp,
    string Password) : IRequest<OnboardingStepResponseDto>;

public sealed class StartOnboardingProfileCommandHandler(
    IRepository<OnboardingSession> onboardingRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<StartOnboardingProfileCommand, OnboardingStepResponseDto>
{
    public async Task<OnboardingStepResponseDto> Handle(StartOnboardingProfileCommand request, CancellationToken cancellationToken)
    {
        var session = new OnboardingSession
        {
            FullName = request.FullName.Trim(),
            Email = request.Email.Trim().ToLowerInvariant(),
            Whatsapp = request.Whatsapp.Trim(),
            Password = request.Password,
            CurrentStep = 1
        };

        await onboardingRepository.AddAsync(session, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new OnboardingStepResponseDto(session.Id, 1, 4, "compliance");
    }
}
