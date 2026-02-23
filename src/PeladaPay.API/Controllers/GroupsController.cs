using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PeladaPay.Application.Features.Groups.Commands;

namespace PeladaPay.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class GroupsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGroupCommand command, CancellationToken cancellationToken)
        => Ok(await mediator.Send(command, cancellationToken));
}
