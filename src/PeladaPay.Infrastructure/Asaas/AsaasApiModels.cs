using System.Text.Json.Serialization;

namespace PeladaPay.Infrastructure.Asaas;

internal sealed record AsaasCreateAccountApiRequest(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("email")] string Email,
    [property: JsonPropertyName("cpfCnpj")] string CpfCnpj,
    [property: JsonPropertyName("mobilePhone")] string? MobilePhone);

internal sealed record AsaasCreateAccountApiResponse([property: JsonPropertyName("id")] string Id);

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
