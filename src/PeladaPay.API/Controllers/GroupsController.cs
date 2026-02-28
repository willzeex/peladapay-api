using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PeladaPay.API.Contracts;
using PeladaPay.Application.DTOs;
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
    [ProducesResponseType(typeof(ApiResponse<GroupDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiValidationErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreateGroupCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, new ApiResponse<GroupDto>(
            StatusCodes.Status201Created,
            "Grupo criado com sucesso.",
            result));
    }


    [HttpPost("{groupId:guid}/asaas-subaccount")]
    [ProducesResponseType(typeof(ApiResponse<AsaasSubaccountDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateAsaasSubaccount(Guid groupId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateAsaasSubaccountCommand(groupId), cancellationToken);
        return StatusCode(StatusCodes.Status200OK, new ApiResponse<AsaasSubaccountDto>(
            StatusCodes.Status200OK,
            result.AlreadyExisted ? "Subconta ASAAS já estava vinculada." : "Subconta ASAAS criada com sucesso.",
            result));
    }

    [HttpPost("{groupId:guid}/players")]
    [ProducesResponseType(typeof(ApiResponse<PlayerDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiValidationErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddPlayer(Guid groupId, [FromBody] AddPlayerToGroupCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command with { GroupId = groupId }, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, new ApiResponse<PlayerDto>(
            StatusCodes.Status201Created,
            "Jogador adicionado ao grupo com sucesso.",
            result));
    }


    [HttpPatch("{groupId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<GroupDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiValidationErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateGroupSettings(Guid groupId, [FromBody] UpdateGroupSettingsRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateGroupSettingsCommand(
            groupId,
            request.Name,
            request.MatchDate,
            request.Frequency,
            request.Venue,
            request.CrestUrl);

        var result = await mediator.Send(command, cancellationToken);
        return StatusCode(StatusCodes.Status200OK, new ApiResponse<GroupDto>(
            StatusCodes.Status200OK,
            "Configurações do grupo atualizadas com sucesso.",
            result));
    }


    [HttpGet("{groupId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<GroupSettingsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetGroupSettings(Guid groupId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetGroupSettingsQuery(groupId), cancellationToken);
        return StatusCode(StatusCodes.Status200OK, new ApiResponse<GroupSettingsDto>(
            StatusCodes.Status200OK,
            "Configurações do grupo consultadas com sucesso.",
            result));
    }

    [HttpGet("my")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<GroupDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetMyGroups(CancellationToken cancellationToken)
    {
        var organizerId = currentUserService.UserId ?? throw new UnauthorizedAccessException();
        var result = await mediator.Send(new GetGroupsByOrganizerQuery(organizerId), cancellationToken);
        return StatusCode(StatusCodes.Status200OK, new ApiResponse<IReadOnlyCollection<GroupDto>>(
            StatusCodes.Status200OK,
            "Grupos do organizador consultados com sucesso.",
            result));
    }

    [HttpGet("{groupId:guid}/players")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<PlayerDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPlayersByGroup(Guid groupId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetPlayersByGroupQuery(groupId), cancellationToken);
        return StatusCode(StatusCodes.Status200OK, new ApiResponse<IReadOnlyCollection<PlayerDto>>(
            StatusCodes.Status200OK,
            "Jogadores do grupo consultados com sucesso.",
            result));
    }

    [HttpGet("players/{playerId:guid}/activities")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<PlayerActivityDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPlayerActivities(Guid playerId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetPlayerActivitiesQuery(playerId), cancellationToken);
        return StatusCode(StatusCodes.Status200OK, new ApiResponse<IReadOnlyCollection<PlayerActivityDto>>(
            StatusCodes.Status200OK,
            "Atividades do jogador consultadas com sucesso.",
            result));
    }
}


public sealed record UpdateGroupSettingsRequest(
    string? Name,
    DateTime? MatchDate,
    string? Frequency,
    string? Venue,
    string? CrestUrl);
