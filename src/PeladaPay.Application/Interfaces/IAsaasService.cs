using PeladaPay.Application.Models.Asaas;

namespace PeladaPay.Application.Interfaces;

public interface IAsaasService
{
    /// <summary>
    /// Cria um cliente no ASAAS para cobrança.
    /// </summary>
    Task<AsaasCreateAccountResponse> CreateSubaccountAsync(AsaasCreateAccountRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Cria uma cobrança PIX para o cliente informado.
    /// </summary>
    Task<AsaasCreatePixPaymentResponse> CreatePixPaymentAsync(AsaasCreatePixPaymentRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Obtém o payload do QR Code PIX para uma cobrança já criada.
    /// </summary>
    Task<string> GetPixQrCodeAsync(string paymentId, CancellationToken cancellationToken);
}
