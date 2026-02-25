using MediatR;
using Microsoft.AspNetCore.Identity;
using PeladaPay.Application.DTOs;
using PeladaPay.Domain.Entities;

namespace PeladaPay.Application.Features.Auth.Commands;

public sealed record StartOnboardingProfileCommand(
    string FullName,
    string Email,
    string Whatsapp,
    string Password) : IRequest<OnboardingStepResponseDto>;

public sealed class StartOnboardingProfileCommandHandler(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager) : IRequestHandler<StartOnboardingProfileCommand, OnboardingStepResponseDto>
{
    private const string OrganizerRole = "Organizer";

    public async Task<OnboardingStepResponseDto> Handle(StartOnboardingProfileCommand request, CancellationToken cancellationToken)
    {
        var user = new ApplicationUser
        {
            UserName = request.Email.Trim().ToLowerInvariant(),
            Email = request.Email.Trim().ToLowerInvariant(),
            FullName = request.FullName.Trim(),
            Whatsapp = request.Whatsapp.Trim(),
            PhoneNumber = request.Whatsapp.Trim(),
            OnboardingCurrentStep = 1
        };

        var creationResult = await userManager.CreateAsync(user, request.Password);
        if (!creationResult.Succeeded)
        {
            var errors = string.Join("; ", creationResult.Errors.Select(x => x.Description));
            throw new InvalidOperationException(errors);
        }

        if (!await roleManager.RoleExistsAsync(OrganizerRole))
        {
            var roleCreationResult = await roleManager.CreateAsync(new IdentityRole(OrganizerRole));
            if (!roleCreationResult.Succeeded)
            {
                var roleErrors = string.Join("; ", roleCreationResult.Errors.Select(x => x.Description));
                throw new InvalidOperationException(roleErrors);
            }
        }

        var addToRoleResult = await userManager.AddToRoleAsync(user, OrganizerRole);
        if (!addToRoleResult.Succeeded)
        {
            var roleErrors = string.Join("; ", addToRoleResult.Errors.Select(x => x.Description));
            throw new InvalidOperationException(roleErrors);
        }

        return new OnboardingStepResponseDto(Guid.Parse(user.Id), 1, 4, "compliance");
    }
}
