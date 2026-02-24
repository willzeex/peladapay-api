using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PeladaPay.Application.Features.Groups.Commands;
using PeladaPay.Application.Features.Groups.Queries;
using PeladaPay.Application.Interfaces;

namespace PeladaPay.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class GroupsController(IMediator mediator, ICurrentUserService currentUserService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGroupCommand command, CancellationToken cancellationToken)
        => Ok(await mediator.Send(command, cancellationToken));

    [HttpPost("{groupId:guid}/players")]
    public async Task<IActionResult> AddPlayer(Guid groupId, [FromBody] AddPlayerToGroupCommand command, CancellationToken cancellationToken)
        => Ok(await mediator.Send(command with { GroupId = groupId }, cancellationToken));

    [HttpGet("my")]
    public async Task<IActionResult> GetMyGroups(CancellationToken cancellationToken)
    {
        var organizerId = currentUserService.UserId ?? throw new UnauthorizedAccessException();
        return Ok(await mediator.Send(new GetGroupsByOrganizerQuery(organizerId), cancellationToken));
    }

    [HttpGet("{groupId:guid}/players")]
    public async Task<IActionResult> GetPlayersByGroup(Guid groupId, CancellationToken cancellationToken)
        => Ok(await mediator.Send(new GetPlayersByGroupQuery(groupId), cancellationToken));

    [HttpGet("players/{playerId:guid}/activities")]
    public async Task<IActionResult> GetPlayerActivities(Guid playerId, CancellationToken cancellationToken)
        => Ok(await mediator.Send(new GetPlayerActivitiesQuery(playerId), cancellationToken));
}
