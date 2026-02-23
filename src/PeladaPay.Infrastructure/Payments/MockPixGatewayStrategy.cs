using PeladaPay.Application.Interfaces;

namespace PeladaPay.Infrastructure.Payments;

public class MockPixGatewayStrategy : IPaymentGatewayStrategy
{
    public Task<(string chargeId, string qrCode)> CreatePixChargeAsync(decimal amount, string payerName, CancellationToken cancellationToken)
    {
        var chargeId = $"charge_{Guid.NewGuid():N}";
        var qrCode = $"00020101021226790014BR.GOV.BCB.PIX0114+55819999999995204000053039865802BR5913{payerName[..Math.Min(12, payerName.Length)]}6008BRASILIA62070503***6304ABCD";
        return Task.FromResult((chargeId, qrCode));
    }
}
