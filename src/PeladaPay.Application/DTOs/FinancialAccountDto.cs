namespace PeladaPay.Application.DTOs;

public sealed record FinancialAccountDto(
    Guid Id,
    decimal Balance,
    string PixKey,
    string ExternalSubaccountId,
    decimal MonthlyFee,
    decimal SingleMatchFee,
    int DueDay,
    bool IsExpenseManagementOnly);
