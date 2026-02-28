using MediatR;
using PeladaPay.Application.DTOs;
using PeladaPay.Application.Interfaces;
using PeladaPay.Domain.Entities;
using PeladaPay.Domain.Exceptions;
using PeladaPay.Domain.Interfaces;

namespace PeladaPay.Application.Features.FinancialAccounts.Queries;

public sealed record GetFinancialAccountSettingsQuery(Guid FinancialAccountId) : IRequest<FinancialAccountDto>;

public sealed class GetFinancialAccountSettingsQueryHandler(
    IRepository<FinancialAccount> financialAccountRepository,
    IRepository<Group> groupRepository,
    ICurrentUserService currentUserService)
    : IRequestHandler<GetFinancialAccountSettingsQuery, FinancialAccountDto>
{
    public async Task<FinancialAccountDto> Handle(GetFinancialAccountSettingsQuery request, CancellationToken cancellationToken)
    {
        var organizerId = currentUserService.UserId ?? throw new UnauthorizedAccessException();

        var ownerGroups = await groupRepository.GetAsync(
            g => g.FinancialAccountId == request.FinancialAccountId && g.OrganizerId == organizerId,
            cancellationToken);

        if (!ownerGroups.Any())
        {
            throw new NotFoundException("Conta financeira não encontrada para o organizador autenticado.");
        }

        var account = await financialAccountRepository.GetByIdAsync(request.FinancialAccountId, cancellationToken)
            ?? throw new NotFoundException("Conta financeira não encontrada.");

        return new FinancialAccountDto(
            account.Id,
            account.Balance,
            account.PixKey,
            account.ExternalSubaccountId,
            account.MonthlyFee,
            account.SingleMatchFee,
            account.DueDay,
            account.IsExpenseManagementOnly);
    }
}
