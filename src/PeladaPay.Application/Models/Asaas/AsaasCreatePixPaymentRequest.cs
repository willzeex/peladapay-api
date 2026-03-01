namespace PeladaPay.Application.Models.Asaas;

public sealed record AsaasCreatePixPaymentRequest(
    string CustomerId,
    decimal Value,
    string Description,
    string ExternalReference,
    DateTime DueDate);
