using MediatR;
using PeladaPay.Application.DTOs;
using PeladaPay.Application.Interfaces;
using PeladaPay.Domain.Entities;
using PeladaPay.Domain.Enums;
using PeladaPay.Domain.Exceptions;
using PeladaPay.Domain.Interfaces;

namespace PeladaPay.Application.Features.Payments.Commands;

public sealed record GeneratePixChargeCommand(Guid GroupId, Guid PlayerId, decimal Amount, string Description) : IRequest<PixChargeDto>;

public sealed class GeneratePixChargeCommandHandler(
    IRepository<Transaction> transactionRepository,
    IRepository<Player> playerRepository,
    IRepository<GroupPlayer> groupPlayerRepository,
    IRepository<FinancialAccount> financialAccountRepository,
    IRepository<Group> groupRepository,
    IPaymentGatewayStrategy paymentGatewayStrategy,
    IUnitOfWork unitOfWork) : IRequestHandler<GeneratePixChargeCommand, PixChargeDto>
{
    /// <summary>
    /// Cria uma cobrança PIX para um jogador específico e persiste o vínculo da transação com o link de pagamento.
    /// </summary>
    public async Task<PixChargeDto> Handle(GeneratePixChargeCommand request, CancellationToken cancellationToken)
    {
        var player = await playerRepository.GetByIdAsync(request.PlayerId, cancellationToken)
            ?? throw new NotFoundException("Jogador não encontrado.");

        var group = await groupRepository.GetByIdAsync(request.GroupId, cancellationToken)
            ?? throw new NotFoundException("Grupo não encontrado.");

        var isPlayerInGroup = (await groupPlayerRepository.GetAsync(
            gp => gp.GroupId == request.GroupId && gp.PlayerId == request.PlayerId,
            cancellationToken)).Any();

        if (!isPlayerInGroup)
            throw new NotFoundException("Jogador não está vinculado ao grupo informado.");

        var financialAccount = await financialAccountRepository.GetByIdAsync(group.FinancialAccountId, cancellationToken)
            ?? throw new NotFoundException("Conta financeira do grupo não encontrada.");

        var (chargeId, qrCode, paymentLink) = await paymentGatewayStrategy.CreatePixChargeAsync(
            financialAccount.ExternalSubaccountId,
            request.Amount,
            player.Name,
            player.Cpf,
            player.Email,
            player.Phone,
            cancellationToken);

        var transaction = new Transaction
        {
            GroupId = request.GroupId,
            PlayerId = request.PlayerId,
            Amount = request.Amount,
            DateUtc = DateTime.UtcNow,
            Type = TransactionType.Entrada,
            Status = TransactionStatus.Pendente,
            ExternalChargeId = chargeId,
            PaymentLink = paymentLink,
            Description = request.Description
        };

        await transactionRepository.AddAsync(transaction, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new PixChargeDto(transaction.Id, request.PlayerId, chargeId, qrCode, paymentLink);
    }
}
