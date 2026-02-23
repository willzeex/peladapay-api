namespace PeladaPay.Application.Interfaces;

public interface IPaymentGatewayStrategy
{
    Task<(string chargeId, string qrCode)> CreatePixChargeAsync(decimal amount, string payerName, CancellationToken cancellationToken);
}
