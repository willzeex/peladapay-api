using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PeladaPay.API.Contracts;
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
    {
        var result = await mediator.Send(command, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, new ApiResponse<object>(
            StatusCodes.Status201Created,
            "Grupo criado com sucesso.",
            result));
    }

    [HttpPost("{groupId:guid}/players")]
    public async Task<IActionResult> AddPlayer(Guid groupId, [FromBody] AddPlayerToGroupCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command with { GroupId = groupId }, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, new ApiResponse<object>(
            StatusCodes.Status201Created,
            "Jogador adicionado ao grupo com sucesso.",
            result));
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyGroups(CancellationToken cancellationToken)
    {
        var organizerId = currentUserService.UserId ?? throw new UnauthorizedAccessException();
        var result = await mediator.Send(new GetGroupsByOrganizerQuery(organizerId), cancellationToken);
        return StatusCode(StatusCodes.Status200OK, new ApiResponse<object>(
            StatusCodes.Status200OK,
            "Grupos do organizador consultados com sucesso.",
            result));
    }

    [HttpGet("{groupId:guid}/players")]
    public async Task<IActionResult> GetPlayersByGroup(Guid groupId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetPlayersByGroupQuery(groupId), cancellationToken);
        return StatusCode(StatusCodes.Status200OK, new ApiResponse<object>(
            StatusCodes.Status200OK,
            "Jogadores do grupo consultados com sucesso.",
            result));
    }

    [HttpGet("players/{playerId:guid}/activities")]
    public async Task<IActionResult> GetPlayerActivities(Guid playerId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetPlayerActivitiesQuery(playerId), cancellationToken);
        return StatusCode(StatusCodes.Status200OK, new ApiResponse<object>(
            StatusCodes.Status200OK,
            "Atividades do jogador consultadas com sucesso.",
            result));
    }
}
