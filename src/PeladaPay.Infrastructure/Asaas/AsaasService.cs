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

    /// <inheritdoc />
    public async Task<AsaasCreateAccountResponse> CreateSubaccountAsync(AsaasCreateAccountRequest request, CancellationToken cancellationToken)
    {
        var (options, correlationId) = ValidateConfiguration();
        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "customers")
        {
            Content = JsonContent.Create(new AsaasCreateAccountApiRequest(
                request.Name,
                request.Email,
                request.CpfCnpj,
                request.MobilePhone))
        };

        AddDefaultHeaders(httpRequest, options.ApiKey, correlationId);

        using var response = await httpClient.SendAsync(httpRequest, cancellationToken);
        await EnsureSuccessResponseAsync(response, correlationId, "criar cliente", cancellationToken);

        var payload = await response.Content.ReadFromJsonAsync<AsaasCreateAccountApiResponse>(JsonOptions, cancellationToken)
            ?? throw new AsaasIntegrationException("Resposta inválida da API ASAAS.");

        if (string.IsNullOrWhiteSpace(payload.Id))
            throw new AsaasIntegrationException("Resposta da API ASAAS não contém id do cliente.");

        return new AsaasCreateAccountResponse(payload.Id);
    }

    /// <inheritdoc />
    public async Task<AsaasCreatePixPaymentResponse> CreatePixPaymentAsync(AsaasCreatePixPaymentRequest request, CancellationToken cancellationToken)
    {
        var (options, correlationId) = ValidateConfiguration();
        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "payments")
        {
            Content = JsonContent.Create(new AsaasCreatePaymentApiRequest(
                request.CustomerId,
                "PIX",
                request.Value,
                DateOnly.FromDateTime(request.DueDate),
                request.Description,
                request.ExternalReference))
        };

        AddDefaultHeaders(httpRequest, options.ApiKey, correlationId);

        using var response = await httpClient.SendAsync(httpRequest, cancellationToken);
        await EnsureSuccessResponseAsync(response, correlationId, "criar cobrança PIX", cancellationToken);

        var payload = await response.Content.ReadFromJsonAsync<AsaasCreatePaymentApiResponse>(JsonOptions, cancellationToken)
            ?? throw new AsaasIntegrationException("Resposta inválida da API ASAAS ao criar cobrança PIX.");

        if (string.IsNullOrWhiteSpace(payload.Id) || string.IsNullOrWhiteSpace(payload.InvoiceUrl))
            throw new AsaasIntegrationException("Resposta da API ASAAS não contém dados essenciais da cobrança PIX.");

        var pixPayload = payload.PixTransaction?.Payload;
        if (string.IsNullOrWhiteSpace(pixPayload))
            pixPayload = await GetPixQrCodeAsync(payload.Id, cancellationToken);

        return new AsaasCreatePixPaymentResponse(payload.Id, pixPayload!, payload.InvoiceUrl);
    }


    /// <inheritdoc />
    public async Task<string> GetPixQrCodeAsync(string paymentId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(paymentId))
            throw new AsaasIntegrationException("Id da cobrança PIX inválido para obtenção do QR Code.");

        var (options, correlationId) = ValidateConfiguration();
        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"payments/{paymentId}/pixQrCode");
        AddDefaultHeaders(httpRequest, options.ApiKey, correlationId);

        using var response = await httpClient.SendAsync(httpRequest, cancellationToken);
        await EnsureSuccessResponseAsync(response, correlationId, "consultar QR Code PIX", cancellationToken);

        var payload = await response.Content.ReadFromJsonAsync<AsaasGetPixQrCodeApiResponse>(JsonOptions, cancellationToken)
            ?? throw new AsaasIntegrationException("Resposta inválida da API ASAAS ao consultar QR Code PIX.");

        if (string.IsNullOrWhiteSpace(payload.Payload))
            throw new AsaasIntegrationException("Resposta da API ASAAS não contém payload PIX.");

        return payload.Payload;
    }

    private (AsaasOptions options, string correlationId) ValidateConfiguration()
    {
        var options = asaasOptions.Value;
        if (string.IsNullOrWhiteSpace(options.ApiKey))
            throw new AsaasIntegrationException("ASAAS API key não configurada.");

        return (options, Guid.NewGuid().ToString("N"));
    }

    private static void AddDefaultHeaders(HttpRequestMessage request, string apiKey, string correlationId)
    {
        request.Headers.UserAgent.ParseAdd("PeladaPay/1.0");
        request.Headers.Add("Accept", "application/json");
        request.Headers.Add("access_token", apiKey);
        request.Headers.Add("X-Correlation-Id", correlationId);
    }

    private async Task EnsureSuccessResponseAsync(HttpResponseMessage response, string correlationId, string operation, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
            return;

        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        string reason = body;

        try
        {
            var parsed = JsonSerializer.Deserialize<AsaasErrorResponse>(body, JsonOptions);
            if (parsed?.Errors.Count > 0)
                reason = string.Join("; ", parsed.Errors.Select(x => $"{x.Code}: {x.Description}"));
        }
        catch
        {
            // mantém mensagem original
        }

        logger.LogError(
            "ASAAS request falhou em {Operation}. StatusCode: {StatusCode}, CorrelationId: {CorrelationId}, Reason: {Reason}",
            operation,
            (int)response.StatusCode,
            correlationId,
            reason);

        throw new AsaasIntegrationException($"Falha ao {operation} no ASAAS: {reason}");
    }
}
