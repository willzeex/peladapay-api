using MediatR;
using Microsoft.AspNetCore.Identity;
using PeladaPay.Application.DTOs;
using PeladaPay.Application.Interfaces;
using PeladaPay.Domain.Constants;
using PeladaPay.Domain.Entities;
using PeladaPay.Domain.Exceptions;
using PeladaPay.Domain.Interfaces;

namespace PeladaPay.Application.Features.Auth.Commands;

public sealed record RegisterManagerCommand(
    string FullName,
    string Cpf,
    string Whatsapp,
    string Phone,
    string Cellphone,
    string Email,
    string Password,
    Guid? PlanId) : IRequest<AuthResponseDto>;

public sealed class RegisterManagerCommandHandler(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    IRepository<Plan> planRepository,
    IJwtTokenGenerator jwtTokenGenerator) : IRequestHandler<RegisterManagerCommand, AuthResponseDto>
{
    private const string OrganizerRole = "Organizer";

    public async Task<AuthResponseDto> Handle(RegisterManagerCommand request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var normalizedWhatsapp = request.Whatsapp.Trim();
        var normalizedPhone = request.Phone.Trim();
        var normalizedCellphone = request.Cellphone.Trim();

        var resolvedPlanId = request.PlanId ?? AvaliablePlans.FreeId;
        var plan = await planRepository.GetByIdAsync(resolvedPlanId, cancellationToken)
            ?? throw new NotFoundException("Plano informado não encontrado.");

        var user = new ApplicationUser
        {
            UserName = normalizedEmail,
            Email = normalizedEmail,
            FullName = request.FullName.Trim(),
            Cpf = request.Cpf.Trim(),
            Whatsapp = normalizedWhatsapp,
            PhoneNumber = string.IsNullOrWhiteSpace(normalizedCellphone)
                ? normalizedPhone
                : normalizedCellphone,
            PlanId = plan.Id
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(x => x.Description));
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

        var token = jwtTokenGenerator.Generate(user);
        return new AuthResponseDto(user.Id, user.Email!, token);
    }
}
