namespace PeladaPay.Application.DTOs;

public sealed record PixChargeDto(Guid TransactionId, string ChargeId, string QrCode);
