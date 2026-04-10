using MediatR;
using Microsoft.AspNetCore.Identity;
using PeladaPay.Application.Interfaces;
using PeladaPay.Domain.Entities;

namespace PeladaPay.Application.Features.Auth.Commands;

public sealed record LogoutManagerCommand : IRequest;

public sealed class LogoutManagerCommandHandler(
    UserManager<ApplicationUser> userManager,
    ICurrentUserService currentUserService) : IRequestHandler<LogoutManagerCommand>
{
    public async Task Handle(LogoutManagerCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("Usuario nao autenticado.");
        var user = await userManager.FindByIdAsync(userId)
            ?? throw new UnauthorizedAccessException("Usuario autenticado nao encontrado.");

        var result = await userManager.UpdateSecurityStampAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(x => x.Description));
            throw new InvalidOperationException(errors);
        }
    }
}
