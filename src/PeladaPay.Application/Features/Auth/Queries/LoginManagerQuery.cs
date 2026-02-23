using MediatR;
using Microsoft.AspNetCore.Identity;
using PeladaPay.Application.DTOs;
using PeladaPay.Application.Interfaces;
using PeladaPay.Domain.Entities;

namespace PeladaPay.Application.Features.Auth.Queries;

public sealed record LoginManagerQuery(string Email, string Password) : IRequest<AuthResponseDto>;

public sealed class LoginManagerQueryHandler(
    UserManager<ApplicationUser> userManager,
    IJwtTokenGenerator jwtTokenGenerator) : IRequestHandler<LoginManagerQuery, AuthResponseDto>
{
    public async Task<AuthResponseDto> Handle(LoginManagerQuery request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email)
            ?? throw new UnauthorizedAccessException("Credenciais inválidas.");

        var validPassword = await userManager.CheckPasswordAsync(user, request.Password);
        if (!validPassword)
        {
            throw new UnauthorizedAccessException("Credenciais inválidas.");
        }

        return new AuthResponseDto(user.Id, user.Email!, jwtTokenGenerator.Generate(user));
    }
}
