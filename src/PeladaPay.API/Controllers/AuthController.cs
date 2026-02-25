using MediatR;
using Microsoft.AspNetCore.Mvc;
using PeladaPay.API.Contracts;
using PeladaPay.Application.Features.Auth.Commands;
using PeladaPay.Application.Features.Auth.Queries;

namespace PeladaPay.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterManagerCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, new ApiResponse<object>(
            StatusCodes.Status201Created,
            "Gestor registrado com sucesso.",
            result));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginManagerQuery query, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return StatusCode(StatusCodes.Status200OK, new ApiResponse<object>(
            StatusCodes.Status200OK,
            "Login realizado com sucesso.",
            result));
    }
}
