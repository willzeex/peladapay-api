using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PeladaPay.API.Contracts;
using PeladaPay.Application.DTOs;
using PeladaPay.Application.Features.Plans.Queries;

namespace PeladaPay.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class PlansController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<PlanDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetPlansQuery(), cancellationToken);
        return StatusCode(StatusCodes.Status200OK, new ApiResponse<IReadOnlyCollection<PlanDto>>(
            StatusCodes.Status200OK,
            "Planos consultados com sucesso.",
            result));
    }
}
