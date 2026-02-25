using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PeladaPay.API.Contracts;
using PeladaPay.Application.DTOs;
using PeladaPay.Application.Features.Dashboard.Queries;

namespace PeladaPay.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class DashboardController(IMediator mediator) : ControllerBase
{
    [HttpGet("{groupId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<DashboardSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetSummary(Guid groupId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetDashboardSummaryQuery(groupId), cancellationToken);
        return StatusCode(StatusCodes.Status200OK, new ApiResponse<DashboardSummaryDto>(
            StatusCodes.Status200OK,
            "Resumo do dashboard consultado com sucesso.",
            result));
    }
}
