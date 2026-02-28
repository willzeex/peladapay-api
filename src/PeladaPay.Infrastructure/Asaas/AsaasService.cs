using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PeladaPay.Application.Exceptions;
using PeladaPay.Application.Interfaces;
using PeladaPay.Application.Models.Asaas;

namespace PeladaPay.Infrastructure.Asaas;

public sealed class AsaasService(
    HttpClient httpClient,
    IOptions<AsaasOptions> asaasOptions,
    ILogger<AsaasService> logger) : IAsaasService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<AsaasCreateAccountResponse> CreateSubaccountAsync(AsaasCreateAccountRequest request, CancellationToken cancellationToken)
    {
        var options = asaasOptions.Value;
        if (string.IsNullOrWhiteSpace(options.ApiKey))
        {
            throw new AsaasIntegrationException("ASAAS API key não configurada.");
        }

        var correlationId = Guid.NewGuid().ToString("N");
        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "accounts")
        {
            Content = JsonContent.Create(new AsaasCreateAccountApiRequest(
                request.Name,
                request.Email,
                request.CpfCnpj,
                request.MobilePhone))
        };

        httpRequest.Headers.Add("access_token", options.ApiKey);
        httpRequest.Headers.Add("X-Correlation-Id", correlationId);

        using var response = await httpClient.SendAsync(httpRequest, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            string reason = body;

            try
            {
                var parsed = JsonSerializer.Deserialize<AsaasErrorResponse>(body, JsonOptions);
                if (parsed?.Errors.Count > 0)
                {
                    reason = string.Join("; ", parsed.Errors.Select(x => $"{x.Code}: {x.Description}"));
                }
            }
            catch
            {
                // keep raw body when parse fails
            }

            logger.LogError(
                "ASAAS create account failed. StatusCode: {StatusCode}, CorrelationId: {CorrelationId}, Reason: {Reason}",
                (int)response.StatusCode,
                correlationId,
                reason);

            throw new AsaasIntegrationException($"Falha ao criar subconta ASAAS: {reason}");
        }

        var payload = await response.Content.ReadFromJsonAsync<AsaasCreateAccountApiResponse>(JsonOptions, cancellationToken)
            ?? throw new AsaasIntegrationException("Resposta inválida da API ASAAS.");

        if (string.IsNullOrWhiteSpace(payload.Id))
        {
            throw new AsaasIntegrationException("Resposta da API ASAAS não contém id da subconta.");
        }

        return new AsaasCreateAccountResponse(payload.Id);
    }
}
