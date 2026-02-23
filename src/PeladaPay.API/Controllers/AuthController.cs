using MediatR;
using Microsoft.AspNetCore.Mvc;
using PeladaPay.Application.Features.Auth.Commands;
using PeladaPay.Application.Features.Auth.Queries;

namespace PeladaPay.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterManagerCommand command, CancellationToken cancellationToken)
        => Ok(await mediator.Send(command, cancellationToken));

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginManagerQuery query, CancellationToken cancellationToken)
        => Ok(await mediator.Send(query, cancellationToken));
}
