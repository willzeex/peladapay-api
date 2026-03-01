using PeladaPay.Application.Interfaces;

namespace PeladaPay.Infrastructure.Payments;

public class MockPixGatewayStrategy : IPaymentGatewayStrategy
{
    /// <inheritdoc />
    public Task<(string chargeId, string qrCode, string paymentLink)> CreatePixChargeAsync(
        decimal amount,
        string payerName,
        string payerCpf,
        string? payerEmail,
        string? payerPhone,
        CancellationToken cancellationToken)
    {
        var chargeId = $"charge_{Guid.NewGuid():N}";
        var qrCode = $"00020101021226790014BR.GOV.BCB.PIX0114+55819999999995204000053039865802BR5913{payerName[..Math.Min(12, payerName.Length)]}6008BRASILIA62070503***6304ABCD";
        var paymentLink = $"https://sandbox.asaas.com/i/{chargeId}";
        return Task.FromResult((chargeId, qrCode, paymentLink));
    }
}
