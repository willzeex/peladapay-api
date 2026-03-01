using System.Text.Json.Serialization;

namespace PeladaPay.Infrastructure.Asaas;

internal sealed record AsaasCreateAccountApiRequest(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("email")] string Email,
    [property: JsonPropertyName("cpfCnpj")] string CpfCnpj,
    [property: JsonPropertyName("mobilePhone")] string? MobilePhone);

internal sealed record AsaasCreateAccountApiResponse([property: JsonPropertyName("id")] string Id);

internal sealed record AsaasCreatePaymentApiRequest(
    [property: JsonPropertyName("customer")] string Customer,
    [property: JsonPropertyName("billingType")] string BillingType,
    [property: JsonPropertyName("value")] decimal Value,
    [property: JsonPropertyName("dueDate")] DateOnly DueDate,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("externalReference")] string ExternalReference);

internal sealed record AsaasCreatePaymentApiResponse(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("invoiceUrl")] string InvoiceUrl,
    [property: JsonPropertyName("pixTransaction")] AsaasPixTransactionApiResponse? PixTransaction);

internal sealed record AsaasPixTransactionApiResponse(
    [property: JsonPropertyName("payload")] string Payload);

internal sealed class AsaasErrorResponse
{
    [JsonPropertyName("errors")]
    public List<AsaasApiError> Errors { get; set; } = [];
}

internal sealed class AsaasApiError
{
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
}
