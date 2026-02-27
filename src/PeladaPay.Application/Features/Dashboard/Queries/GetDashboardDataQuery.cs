using MediatR;
using PeladaPay.Application.DTOs;
using PeladaPay.Application.Interfaces;
using PeladaPay.Domain.Entities;
using PeladaPay.Domain.Enums;
using PeladaPay.Domain.Interfaces;

namespace PeladaPay.Application.Features.Dashboard.Queries;

public sealed record GetDashboardDataQuery(Guid GroupId) : IRequest<DashboardDataDto>;

public sealed class GetDashboardDataQueryHandler(
    IRepository<Group> groupRepository,
    IRepository<FinancialAccount> accountRepository,
    IRepository<Transaction> transactionRepository,
    ICurrentUserService currentUserService) : IRequestHandler<GetDashboardDataQuery, DashboardDataDto>
{
    public async Task<DashboardDataDto> Handle(GetDashboardDataQuery request, CancellationToken cancellationToken)
    {
        var managerId = currentUserService.UserId ?? throw new UnauthorizedAccessException();
        var group = await groupRepository.GetByIdAsync(request.GroupId, cancellationToken)
            ?? throw new InvalidOperationException("Grupo não encontrado.");

        if (group.OrganizerId != managerId)
        {
            throw new UnauthorizedAccessException("Acesso negado ao grupo.");
        }

        var account = await accountRepository.GetByIdAsync(group.FinancialAccountId, cancellationToken)
            ?? throw new InvalidOperationException("Conta financeira não encontrada.");

        var transactions = await transactionRepository.GetAsync(x => x.GroupId == request.GroupId, cancellationToken);

        var receivables = transactions
            .Where(x => x.Status == TransactionStatus.Pendente && x.Type == TransactionType.Entrada)
            .Sum(x => x.Amount);

        var payables = transactions
            .Where(x => x.Status == TransactionStatus.Pendente && x.Type == TransactionType.Saida)
            .Sum(x => x.Amount);

        var chart = transactions
            .OrderBy(x => x.DateUtc)
            .GroupBy(x => new DateTime(x.DateUtc.Year, x.DateUtc.Month, 1))
            .Select(g => new DashboardChartPointDto(
                g.Key.ToString("MMM/yy"),
                g.Where(x => x.Type == TransactionType.Entrada).Sum(x => x.Amount),
                g.Where(x => x.Type == TransactionType.Saida).Sum(x => x.Amount)))
            .ToArray();

        var recentTransactions = transactions
            .OrderByDescending(x => x.DateUtc)
            .Take(10)
            .Select(x => new DashboardTransactionDto(
                x.Id,
                x.Type == TransactionType.Entrada ? "entrada" : "saida",
                x.Description,
                x.Amount,
                x.DateUtc))
            .ToArray();

        var lastTransactions = transactions
            .OrderByDescending(x => x.DateUtc)
            .Take(5)
            .Select(x => new RecentTransactionDto(
                x.Id,
                x.Amount,
                x.Type.ToString(),
                x.Status.ToString(),
                x.DateUtc,
                x.Description))
            .ToArray();

        return new DashboardDataDto(
            account.Balance,
            receivables,
            payables,
            chart,
            recentTransactions,
            lastTransactions,
            receivables + payables);
    }
}
