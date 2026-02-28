using MediatR;
using Microsoft.AspNetCore.Identity;
using PeladaPay.Application.DTOs;
using PeladaPay.Application.Interfaces;
using PeladaPay.Domain.Entities;

namespace PeladaPay.Application.Features.Users.Queries;

public sealed record GetUserProfileQuery : IRequest<ApplicationUserProfileDto>;

public sealed class GetUserProfileQueryHandler(
    UserManager<ApplicationUser> userManager,
    ICurrentUserService currentUserService) : IRequestHandler<GetUserProfileQuery, ApplicationUserProfileDto>
{
    public async Task<ApplicationUserProfileDto> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException();
        var user = await userManager.FindByIdAsync(userId)
            ?? throw new InvalidOperationException("Usuário autenticado não encontrado.");

        return new ApplicationUserProfileDto(
            user.FullName,
            user.Email ?? string.Empty,
            user.Whatsapp ?? string.Empty,
            user.Cpf ?? string.Empty,
            user.BirthDate?.ToString("yyyy-MM-dd") ?? string.Empty,
            user.Address ?? string.Empty);
    }
}
