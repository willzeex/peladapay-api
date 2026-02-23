namespace PeladaPay.Application.DTOs;

public sealed record DashboardSummaryDto(
    decimal Balance,
    decimal PendingAmount,
    IReadOnlyCollection<RecentTransactionDto> LastTransactions);

public sealed record RecentTransactionDto(Guid Id, decimal Amount, string Type, string Status, DateTime DateUtc, string Description);
