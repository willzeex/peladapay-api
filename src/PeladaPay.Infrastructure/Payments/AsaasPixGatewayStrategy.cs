using PeladaPay.Application.Interfaces;
using PeladaPay.Application.Models.Asaas;

namespace PeladaPay.Infrastructure.Payments;

public sealed class AsaasPixGatewayStrategy(IAsaasService asaasService) : IPaymentGatewayStrategy
{
    /// <inheritdoc />
    public async Task<(string chargeId, string qrCode, string paymentLink)> CreatePixChargeAsync(
        decimal amount,
        string payerName,
        string payerCpf,
        string? payerEmail,
        CancellationToken cancellationToken)
    {
        var createCustomerResponse = await asaasService.CreateSubaccountAsync(
            new AsaasCreateAccountRequest(
                payerName,
                payerEmail ?? $"{Guid.NewGuid():N}@peladapay.local",
                SanitizeDocument(payerCpf),
                null),
            cancellationToken);

        var createPaymentResponse = await asaasService.CreatePixPaymentAsync(
            new AsaasCreatePixPaymentRequest(
                createCustomerResponse.SubaccountId,
                amount,
                $"Pagamento PeladaPay - {payerName}",
                Guid.NewGuid().ToString("N"),
                DateTime.UtcNow.Date),
            cancellationToken);

        return (createPaymentResponse.PaymentId, createPaymentResponse.PixCode, createPaymentResponse.InvoiceUrl);
    }

    private static string SanitizeDocument(string document)
        => new(document.Where(char.IsDigit).ToArray());
}
