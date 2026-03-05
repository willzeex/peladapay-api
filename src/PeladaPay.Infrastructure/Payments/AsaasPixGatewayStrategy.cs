using PeladaPay.Application.Interfaces;
using PeladaPay.Application.Models.Asaas;

namespace PeladaPay.Infrastructure.Payments;

public sealed class AsaasPixGatewayStrategy(IAsaasService asaasService) : IPaymentGatewayStrategy
{
    /// <inheritdoc />  
    public async Task<(string chargeId, string qrCode, string paymentLink)> CreatePixChargeAsync(
        string customerId,
        decimal amount,
        string payerName,
        string? payerCpf, // Updated to match the nullability of the interface  
        string? payerEmail,
        string? payerPhone,
        CancellationToken cancellationToken)
    {
        var createPaymentResponse = await asaasService.CreatePixPaymentAsync(
            new AsaasCreatePixPaymentRequest(
                customerId,
                amount,
                $"Pagamento PeladaPay - {payerName}",
                Guid.NewGuid().ToString("N"),
                DateTime.UtcNow.Date),
            cancellationToken);

        return (createPaymentResponse.PaymentId, createPaymentResponse.PixCode, createPaymentResponse.InvoiceUrl);
    }
}
