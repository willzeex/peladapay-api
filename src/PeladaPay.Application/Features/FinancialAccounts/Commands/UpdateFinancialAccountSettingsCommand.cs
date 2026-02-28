using MediatR;
using PeladaPay.Application.DTOs;
using PeladaPay.Application.Interfaces;
using PeladaPay.Domain.Entities;
using PeladaPay.Domain.Exceptions;
using PeladaPay.Domain.Interfaces;

namespace PeladaPay.Application.Features.FinancialAccounts.Commands;

public sealed record UpdateFinancialAccountSettingsCommand(
    Guid FinancialAccountId,
    string? PixKey,
    decimal? MonthlyFee,
    decimal? SingleMatchFee,
    int? DueDay,
    bool? IsExpenseManagementOnly) : IRequest<FinancialAccountDto>;

public sealed class UpdateFinancialAccountSettingsCommandHandler(
    IRepository<FinancialAccount> financialAccountRepository,
    IRepository<Group> groupRepository,
    ICurrentUserService currentUserService,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateFinancialAccountSettingsCommand, FinancialAccountDto>
{
    public async Task<FinancialAccountDto> Handle(UpdateFinancialAccountSettingsCommand request, CancellationToken cancellationToken)
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

        var hasChanges = false;

        if (request.PixKey is not null)
        {
            account.PixKey = request.PixKey.Trim();
            hasChanges = true;
        }

        if (request.MonthlyFee.HasValue)
        {
            account.MonthlyFee = request.MonthlyFee.Value;
            hasChanges = true;
        }

        if (request.SingleMatchFee.HasValue)
        {
            account.SingleMatchFee = request.SingleMatchFee.Value;
            hasChanges = true;
        }

        if (request.DueDay.HasValue)
        {
            account.DueDay = request.DueDay.Value;
            hasChanges = true;
        }

        if (request.IsExpenseManagementOnly.HasValue)
        {
            account.IsExpenseManagementOnly = request.IsExpenseManagementOnly.Value;
            hasChanges = true;
        }

        if (hasChanges)
        {
            financialAccountRepository.Update(account);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

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
