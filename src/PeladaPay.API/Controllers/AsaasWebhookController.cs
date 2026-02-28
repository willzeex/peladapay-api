using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PeladaPay.API.Contracts;

namespace PeladaPay.API.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/webhooks/asaas")]
public class AsaasWebhookController(ILogger<AsaasWebhookController> logger) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public IActionResult Receive([FromBody] object payload)
    {
        logger.LogInformation("ASAAS webhook received: {@Payload}", payload);

        return Ok(new ApiResponse<object>(
            StatusCodes.Status200OK,
            "Webhook ASAAS recebido com sucesso.",
            payload));
    }
}
