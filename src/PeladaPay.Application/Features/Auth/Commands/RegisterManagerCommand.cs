using MediatR;
using Microsoft.AspNetCore.Identity;
using PeladaPay.Application.DTOs;
using PeladaPay.Application.Interfaces;
using PeladaPay.Domain.Entities;

namespace PeladaPay.Application.Features.Auth.Commands;

public sealed record RegisterManagerCommand(string FullName, string Email, string Password) : IRequest<AuthResponseDto>;

public sealed class RegisterManagerCommandHandler(
    UserManager<ApplicationUser> userManager,
    IJwtTokenGenerator jwtTokenGenerator) : IRequestHandler<RegisterManagerCommand, AuthResponseDto>
{
    public async Task<AuthResponseDto> Handle(RegisterManagerCommand request, CancellationToken cancellationToken)
    {
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(x => x.Description));
            throw new InvalidOperationException(errors);
        }

        var token = jwtTokenGenerator.Generate(user);
        return new AuthResponseDto(user.Id, user.Email!, token);
    }
}
