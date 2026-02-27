namespace PeladaPay.Application.DTOs;

public sealed record DashboardDataDto(
    decimal Balance,
    decimal Receivables,
    decimal Payables,
    IReadOnlyCollection<DashboardChartPointDto> Chart,
    IReadOnlyCollection<DashboardTransactionDto> RecentTransactions,
    IReadOnlyCollection<RecentTransactionDto> LastTransactions,
    decimal PendingAmount);

public sealed record DashboardChartPointDto(string Name, decimal Entradas, decimal Saidas);

public sealed record DashboardTransactionDto(Guid Id, string Type, string Description, decimal Amount, DateTime Date);
