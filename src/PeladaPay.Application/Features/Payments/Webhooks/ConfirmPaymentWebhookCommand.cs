using MediatR;
using PeladaPay.Domain.Entities;
using PeladaPay.Domain.Enums;
using PeladaPay.Domain.Exceptions;
using PeladaPay.Domain.Interfaces;

namespace PeladaPay.Application.Features.Payments.Webhooks;

public sealed record ConfirmPaymentWebhookCommand(string ChargeId, string? ReceiptUrl) : IRequest;

public sealed class ConfirmPaymentWebhookCommandHandler(
    IRepository<Transaction> transactionRepository,
    IRepository<Group> groupRepository,
    IRepository<FinancialAccount> accountRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<ConfirmPaymentWebhookCommand>
{
    /// <summary>
    /// Confirma o pagamento da cobrança e credita o valor na conta financeira do grupo.
    /// </summary>
    public async Task Handle(ConfirmPaymentWebhookCommand request, CancellationToken cancellationToken)
    {
        var transaction = (await transactionRepository.GetAsync(t => t.ExternalChargeId == request.ChargeId, cancellationToken)).FirstOrDefault()
            ?? throw new NotFoundException("Cobrança não encontrada.");

        if (transaction.Status is TransactionStatus.Confirmada)
            return;

        transaction.Status = TransactionStatus.Confirmada;
        if (!string.IsNullOrWhiteSpace(request.ReceiptUrl))
            transaction.ReceiptUrl = request.ReceiptUrl;
        transactionRepository.Update(transaction);

        var group = await groupRepository.GetByIdAsync(transaction.GroupId, cancellationToken)
            ?? throw new NotFoundException("Grupo não encontrado.");
        var account = await accountRepository.GetByIdAsync(group.FinancialAccountId, cancellationToken)
            ?? throw new NotFoundException("Conta financeira não encontrada.");

        account.Balance += transaction.Amount;
        accountRepository.Update(account);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
