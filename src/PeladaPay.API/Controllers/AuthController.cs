using MediatR;
using Microsoft.AspNetCore.Mvc;
using PeladaPay.API.Contracts;
using PeladaPay.Application.DTOs;
using PeladaPay.Application.Features.Auth.Commands;
using PeladaPay.Application.Features.Auth.Queries;

namespace PeladaPay.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiValidationErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register([FromBody] RegisterManagerCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, new ApiResponse<AuthResponseDto>(
            StatusCodes.Status201Created,
            "Gestor registrado com sucesso.",
            result));
    }

    [HttpPost("onboarding/profile")]
    [ProducesResponseType(typeof(ApiResponse<OnboardingStepResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiValidationErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> StartOnboardingProfile([FromBody] StartOnboardingProfileCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, new ApiResponse<OnboardingStepResponseDto>(
            StatusCodes.Status201Created,
            "Etapa de perfil concluída com sucesso.",
            result));
    }

    [HttpPut("onboarding/{sessionId:guid}/compliance")]
    [ProducesResponseType(typeof(ApiResponse<OnboardingStepResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiValidationErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CompleteOnboardingCompliance(Guid sessionId, [FromBody] CompleteOnboardingComplianceRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CompleteOnboardingComplianceCommand(sessionId, request.Cpf, request.BirthDate, request.Address), cancellationToken);
        return StatusCode(StatusCodes.Status200OK, new ApiResponse<OnboardingStepResponseDto>(
            StatusCodes.Status200OK,
            "Etapa de compliance concluída com sucesso.",
            result));
    }

    [HttpPut("onboarding/{sessionId:guid}/pelada")]
    [ProducesResponseType(typeof(ApiResponse<OnboardingStepResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiValidationErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CompleteOnboardingGroup(Guid sessionId, [FromBody] CompleteOnboardingGroupRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CompleteOnboardingGroupCommand(sessionId, request.GroupName, request.Frequency, request.Venue, request.CrestUrl), cancellationToken);
        return StatusCode(StatusCodes.Status200OK, new ApiResponse<OnboardingStepResponseDto>(
            StatusCodes.Status200OK,
            "Etapa de pelada concluída com sucesso.",
            result));
    }

    [HttpPost("onboarding/{sessionId:guid}/financial")]
    [ProducesResponseType(typeof(ApiResponse<OnboardingResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiValidationErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CompleteOnboardingFinancial(Guid sessionId, [FromBody] CompleteOnboardingFinancialRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CompleteOnboardingFinancialCommand(sessionId, request.MonthlyFee, request.SingleMatchFee, request.DueDay, request.IsExpenseManagementOnly), cancellationToken);
        return StatusCode(StatusCodes.Status201Created, new ApiResponse<OnboardingResponseDto>(
            StatusCodes.Status201Created,
            "Fluxo de cadastro concluído com sucesso.",
            result));
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiValidationErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromBody] LoginManagerQuery query, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return StatusCode(StatusCodes.Status200OK, new ApiResponse<AuthResponseDto>(
            StatusCodes.Status200OK,
            "Login realizado com sucesso.",
            result));
    }
}

public sealed record CompleteOnboardingComplianceRequest(string Cpf, DateOnly BirthDate, string Address);
public sealed record CompleteOnboardingGroupRequest(string GroupName, string Frequency, string? Venue, string? CrestUrl);
public sealed record CompleteOnboardingFinancialRequest(decimal MonthlyFee, decimal SingleMatchFee, int DueDay, bool IsExpenseManagementOnly);
