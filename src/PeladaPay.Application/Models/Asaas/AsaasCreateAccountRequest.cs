namespace PeladaPay.Application.Models.Asaas;

public sealed record AsaasCreateAccountRequest(
    string Name,
    string Email,
    string CpfCnpj,
    string? MobilePhone);
