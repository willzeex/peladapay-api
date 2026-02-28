using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PeladaPay.API.Contracts;
using PeladaPay.Application.DTOs;
using PeladaPay.Application.Features.FinancialAccounts.Commands;
using PeladaPay.Application.Features.FinancialAccounts.Queries;

namespace PeladaPay.API.Controllers;

[ApiController]
[Authorize]
[Route("api/financial-accounts")]
public class FinancialAccountsController(IMediator mediator) : ControllerBase
{
    [HttpGet("{financialAccountId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<FinancialAccountDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetFinancialSettings(Guid financialAccountId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetFinancialAccountSettingsQuery(financialAccountId), cancellationToken);
        return StatusCode(StatusCodes.Status200OK, new ApiResponse<FinancialAccountDto>(
            StatusCodes.Status200OK,
            "Configurações financeiras consultadas com sucesso.",
            result));
    }

    [HttpPatch("{financialAccountId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<FinancialAccountDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiValidationErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateFinancialSettings(
        Guid financialAccountId,
        [FromBody] UpdateFinancialAccountSettingsRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateFinancialAccountSettingsCommand(
            financialAccountId,
            request.PixKey,
            request.MonthlyFee,
            request.SingleMatchFee,
            request.DueDay,
            request.IsExpenseManagementOnly);

        var result = await mediator.Send(command, cancellationToken);
        return StatusCode(StatusCodes.Status200OK, new ApiResponse<FinancialAccountDto>(
            StatusCodes.Status200OK,
            "Configurações financeiras atualizadas com sucesso.",
            result));
    }
}

public sealed record UpdateFinancialAccountSettingsRequest(
    string? PixKey,
    decimal? MonthlyFee,
    decimal? SingleMatchFee,
    int? DueDay,
    bool? IsExpenseManagementOnly);
