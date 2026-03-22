namespace PeladaPay.Application.DTOs;

public sealed record PlanDto(
    Guid Id,
    string Name,
    string Slug,
    decimal MonthlyPrice,
    decimal PixFeePercentage,
    int FreeWithdrawalsPerMonth,
    decimal AdditionalWithdrawalFee,
    bool IsPopular,
    IReadOnlyCollection<string> Features);
