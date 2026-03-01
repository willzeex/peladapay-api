namespace PeladaPay.Application.Interfaces;

public interface IPaymentGatewayStrategy
{
    /// <summary>
    /// Cria uma cobrança PIX e retorna os dados para pagamento do jogador.
    /// </summary>
    Task<(string chargeId, string qrCode, string paymentLink)> CreatePixChargeAsync(
        decimal amount,
        string payerName,
        string payerCpf,
        string? payerEmail,
        CancellationToken cancellationToken);
}
