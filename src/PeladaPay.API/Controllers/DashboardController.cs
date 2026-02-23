using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PeladaPay.Application.Features.Dashboard.Queries;

namespace PeladaPay.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class DashboardController(IMediator mediator) : ControllerBase
{
    [HttpGet("{groupId:guid}")]
    public async Task<IActionResult> GetSummary(Guid groupId, CancellationToken cancellationToken)
        => Ok(await mediator.Send(new GetDashboardSummaryQuery(groupId), cancellationToken));
}
