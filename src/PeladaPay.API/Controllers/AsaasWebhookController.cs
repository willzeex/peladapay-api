using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PeladaPay.API.Contracts;
using PeladaPay.Application.Features.Payments.Webhooks;

namespace PeladaPay.API.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/webhooks/asaas")]
public class AsaasWebhookController(IMediator mediator, ILogger<AsaasWebhookController> logger) : ControllerBase
{
    /// <summary>
    /// Recebe eventos do ASAAS e confirma pagamentos PIX quando a cobrança é recebida.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Receive([FromBody] AsaasWebhookPayload payload, CancellationToken cancellationToken)
    {
        if (!string.Equals(payload.Event, "PAYMENT_RECEIVED", StringComparison.OrdinalIgnoreCase) || string.IsNullOrWhiteSpace(payload.Payment?.Id))
        {
            logger.LogInformation("Evento ASAAS ignorado: {Event}", payload.Event);
            return Ok(new ApiResponse<object>(StatusCodes.Status200OK, "Evento ignorado.", null));
        }

        var receiptUrl = payload.Payment.InvoiceUrl ?? string.Empty;
        await mediator.Send(new ConfirmPaymentWebhookCommand(payload.Payment.Id, receiptUrl), cancellationToken);

        return Ok(new ApiResponse<object>(
            StatusCodes.Status200OK,
            "Pagamento confirmado com sucesso.",
            new { payload.Event, payload.Payment.Id }));
    }
}

public sealed record AsaasWebhookPayload(string Event, AsaasWebhookPaymentPayload? Payment);

public sealed record AsaasWebhookPaymentPayload(string Id, string? InvoiceUrl);
