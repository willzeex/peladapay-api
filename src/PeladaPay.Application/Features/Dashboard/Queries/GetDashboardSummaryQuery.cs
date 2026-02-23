using MediatR;
using PeladaPay.Application.DTOs;
using PeladaPay.Application.Interfaces;
using PeladaPay.Domain.Entities;
using PeladaPay.Domain.Enums;
using PeladaPay.Domain.Interfaces;

namespace PeladaPay.Application.Features.Dashboard.Queries;

public sealed record GetDashboardSummaryQuery(Guid GroupId) : IRequest<DashboardSummaryDto>;

public sealed class GetDashboardSummaryQueryHandler(
    IRepository<Group> groupRepository,
    IRepository<FinancialAccount> accountRepository,
    IRepository<Transaction> transactionRepository,
    ICurrentUserService currentUserService) : IRequestHandler<GetDashboardSummaryQuery, DashboardSummaryDto>
{
    public async Task<DashboardSummaryDto> Handle(GetDashboardSummaryQuery request, CancellationToken cancellationToken)
    {
        var managerId = currentUserService.UserId ?? throw new UnauthorizedAccessException();
        var group = await groupRepository.GetByIdAsync(request.GroupId, cancellationToken)
            ?? throw new InvalidOperationException("Grupo não encontrado.");

        if (group.ManagerId != managerId)
        {
            throw new UnauthorizedAccessException("Acesso negado ao grupo.");
        }

        var account = await accountRepository.GetByIdAsync(group.FinancialAccountId, cancellationToken)
            ?? throw new InvalidOperationException("Conta financeira não encontrada.");

        var transactions = await transactionRepository.GetAsync(x => x.GroupId == request.GroupId, cancellationToken);

        var pending = transactions
            .Where(x => x.Status == TransactionStatus.Pendente)
            .Sum(x => x.Amount);

        var last = transactions
            .OrderByDescending(x => x.DateUtc)
            .Take(5)
            .Select(x => new RecentTransactionDto(x.Id, x.Amount, x.Type.ToString(), x.Status.ToString(), x.DateUtc, x.Description))
            .ToArray();

        return new DashboardSummaryDto(account.Balance, pending, last);
    }
}
