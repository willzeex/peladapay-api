using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        => Ok(await mediator.Send(command, cancellationToken));

    [HttpPost("webhook")]
    [AllowAnonymous]
    public async Task<IActionResult> Webhook([FromBody] ConfirmPaymentWebhookCommand command, CancellationToken cancellationToken)
    {
        await mediator.Send(command, cancellationToken);
        return Ok(new { message = "Pagamento confirmado" });
    }
}
