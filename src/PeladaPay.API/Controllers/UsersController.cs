using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PeladaPay.API.Contracts;
using PeladaPay.Application.DTOs;
using PeladaPay.Application.Features.Users.Commands;
using PeladaPay.Application.Features.Users.Queries;

namespace PeladaPay.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class UsersController(IMediator mediator) : ControllerBase
{

    [HttpGet("profile")]
    [ProducesResponseType(typeof(ApiResponse<ApplicationUserProfileDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetUserProfileQuery(), cancellationToken);
        return StatusCode(StatusCodes.Status200OK, new ApiResponse<ApplicationUserProfileDto>(
            StatusCodes.Status200OK,
            "Perfil consultado com sucesso.",
            result));
    }

    [HttpPatch("profile")]
    [ProducesResponseType(typeof(ApiResponse<UserProfileDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiValidationErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return StatusCode(StatusCodes.Status200OK, new ApiResponse<UserProfileDto>(
            StatusCodes.Status200OK,
            "Perfil atualizado com sucesso.",
            result));
    }

    [HttpPatch("onboarding")]
    [ProducesResponseType(typeof(ApiResponse<UpdateUserOnboardingSettingsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiValidationErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateOnboarding([FromBody] UpdateUserOnboardingSettingsCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return StatusCode(StatusCodes.Status200OK, new ApiResponse<UpdateUserOnboardingSettingsResponse>(
            StatusCodes.Status200OK,
            "Configurações de onboarding atualizadas com sucesso.",
            result));
    }
}
