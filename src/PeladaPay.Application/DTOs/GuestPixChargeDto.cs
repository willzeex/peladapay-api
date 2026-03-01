namespace PeladaPay.Application.DTOs;

/// <summary>
/// Representa os dados de uma cobrança PIX criada para um convidado sem vínculo prévio no grupo.
/// </summary>
/// <param name="TransactionId">Identificador da transação criada internamente.</param>
/// <param name="GroupId">Identificador do grupo responsável pela cobrança.</param>
/// <param name="GuestName">Nome do convidado pagador.</param>
/// <param name="ChargeId">Identificador da cobrança no gateway externo.</param>
/// <param name="QrCode">Código PIX para pagamento.</param>
/// <param name="PaymentLink">URL da fatura/pagamento retornada pelo gateway.</param>
public sealed record GuestPixChargeDto(
    Guid TransactionId,
    Guid GroupId,
    string GuestName,
    string ChargeId,
    string QrCode,
    string PaymentLink);
