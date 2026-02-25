using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PeladaPay.API.Contracts;
using PeladaPay.Application.Features.Payments.Commands;
using PeladaPay.Application.Features.Payments.Webhooks;

namespace PeladaPay.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController(IMediator mediator) : ControllerBase
{
    [HttpPost("pix")]
    [Authorize]
    public async Task<IActionResult> GeneratePix([FromBody] GeneratePixChargeCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, new ApiResponse<object>(
            StatusCodes.Status201Created,
            "Cobrança PIX gerada com sucesso.",
            result));
    }

    [HttpPost("webhook")]
    [AllowAnonymous]
    public async Task<IActionResult> Webhook([FromBody] ConfirmPaymentWebhookCommand command, CancellationToken cancellationToken)
    {
        await mediator.Send(command, cancellationToken);
        return StatusCode(StatusCodes.Status200OK, new ApiResponse<object>(
            StatusCodes.Status200OK,
            "Pagamento confirmado.",
            null));
    }
}
