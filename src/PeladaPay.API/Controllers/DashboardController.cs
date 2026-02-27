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

    [HttpGet("{groupId:guid}/balance")]
    [ProducesResponseType(typeof(ApiResponse<decimal>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBalance(Guid groupId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetDashboardDataQuery(groupId), cancellationToken);
        return StatusCode(StatusCodes.Status200OK, new ApiResponse<decimal>(
            StatusCodes.Status200OK,
            "Saldo consultado com sucesso.",
            result.Balance));
    }

    [HttpGet("{groupId:guid}/receivables")]
    [ProducesResponseType(typeof(ApiResponse<decimal>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReceivables(Guid groupId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetDashboardDataQuery(groupId), cancellationToken);
        return StatusCode(StatusCodes.Status200OK, new ApiResponse<decimal>(
            StatusCodes.Status200OK,
            "Valores a receber consultados com sucesso.",
            result.Receivables));
    }

    [HttpGet("{groupId:guid}/payables")]
    [ProducesResponseType(typeof(ApiResponse<decimal>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPayables(Guid groupId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetDashboardDataQuery(groupId), cancellationToken);
        return StatusCode(StatusCodes.Status200OK, new ApiResponse<decimal>(
            StatusCodes.Status200OK,
            "Valores a pagar consultados com sucesso.",
            result.Payables));
    }

    [HttpGet("{groupId:guid}/monthly-health")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<DashboardChartPointDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMonthlyHealth(Guid groupId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetDashboardDataQuery(groupId), cancellationToken);
        return StatusCode(StatusCodes.Status200OK, new ApiResponse<IReadOnlyCollection<DashboardChartPointDto>>(
            StatusCodes.Status200OK,
            "Saúde mensal consultada com sucesso.",
            result.Chart));
    }

    [HttpGet("{groupId:guid}/transactions/recent")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<DashboardTransactionDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRecentTransactions(Guid groupId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetDashboardDataQuery(groupId), cancellationToken);
        return StatusCode(StatusCodes.Status200OK, new ApiResponse<IReadOnlyCollection<DashboardTransactionDto>>(
            StatusCodes.Status200OK,
            "Transações recentes consultadas com sucesso.",
            result.RecentTransactions));
    }
}
