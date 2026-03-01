namespace PeladaPay.Application.Models.Asaas;

public sealed record AsaasCreatePixPaymentResponse(string PaymentId, string PixCode, string InvoiceUrl);
