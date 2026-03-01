namespace PeladaPay.Application.DTOs;

public sealed record PixChargeDto(Guid TransactionId, Guid PlayerId, string ChargeId, string QrCode, string PaymentLink);
