using MediatR;
using Microsoft.AspNetCore.Identity;
using PeladaPay.Application.DTOs;
using PeladaPay.Application.Interfaces;
using PeladaPay.Domain.Entities;

namespace PeladaPay.Application.Features.Users.Commands;

public sealed record UpdateUserProfileCommand(
    string? FullName,
    string? Email,
    string? Whatsapp,
    string? Cpf,
    DateOnly? BirthDate,
    string? Address) : IRequest<UserProfileDto>;

public sealed class UpdateUserProfileCommandHandler(
    UserManager<ApplicationUser> userManager,
    ICurrentUserService currentUserService) : IRequestHandler<UpdateUserProfileCommand, UserProfileDto>
{
    public async Task<UserProfileDto> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException();
        var user = await userManager.FindByIdAsync(userId)
            ?? throw new InvalidOperationException("Usuário autenticado não encontrado.");

        var hasChanges = false;

        if (request.FullName is not null)
        {
            user.FullName = request.FullName.Trim();
            hasChanges = true;
        }

        if (request.Email is not null)
        {
            var normalizedEmail = request.Email.Trim().ToLowerInvariant();
            user.Email = normalizedEmail;
            user.UserName = normalizedEmail;
            hasChanges = true;
        }

        if (request.Whatsapp is not null)
        {
            var sanitizedWhatsapp = string.IsNullOrWhiteSpace(request.Whatsapp)
                ? null
                : request.Whatsapp.Trim();
            user.Whatsapp = sanitizedWhatsapp;
            user.PhoneNumber = sanitizedWhatsapp;
            hasChanges = true;
        }

        if (request.Cpf is not null)
        {
            user.Cpf = string.IsNullOrWhiteSpace(request.Cpf)
                ? null
                : request.Cpf.Trim();
            hasChanges = true;
        }

        if (request.BirthDate.HasValue)
        {
            user.BirthDate = request.BirthDate.Value;
            hasChanges = true;
        }

        if (request.Address is not null)
        {
            user.Address = string.IsNullOrWhiteSpace(request.Address)
                ? null
                : request.Address.Trim();
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

        return new UserProfileDto(user.FullName, user.Email ?? string.Empty, user.Whatsapp, user.Cpf, user.BirthDate, user.Address);
    }
}
