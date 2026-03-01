using MediatR;
using PeladaPay.Application.DTOs;
using PeladaPay.Application.Interfaces;
using PeladaPay.Domain.Entities;
using PeladaPay.Domain.Enums;
using PeladaPay.Domain.Exceptions;
using PeladaPay.Domain.Interfaces;

namespace PeladaPay.Application.Features.Payments.Commands;

public sealed record GenerateGuestPixChargeCommand(
    Guid GroupId,
    string GuestName,
    string GuestCpf,
    string? GuestEmail,
    decimal Amount,
    string Description) : IRequest<GuestPixChargeDto>;

public sealed class GenerateGuestPixChargeCommandHandler(
    IRepository<Group> groupRepository,
    IRepository<Transaction> transactionRepository,
    IPaymentGatewayStrategy paymentGatewayStrategy,
    IUnitOfWork unitOfWork)
    : IRequestHandler<GenerateGuestPixChargeCommand, GuestPixChargeDto>
{
    /// <summary>
    /// Cria uma cobrança PIX para um convidado sem cadastro prévio no grupo e persiste a transação para rastreabilidade.
    /// </summary>
    public async Task<GuestPixChargeDto> Handle(GenerateGuestPixChargeCommand request, CancellationToken cancellationToken)
    {
        var group = await groupRepository.GetByIdAsync(request.GroupId, cancellationToken)
            ?? throw new NotFoundException("Grupo não encontrado.");

        var (chargeId, qrCode, paymentLink) = await paymentGatewayStrategy.CreatePixChargeAsync(
            request.Amount,
            request.GuestName,
            request.GuestCpf,
            request.GuestEmail,
            cancellationToken);

        var transaction = new Transaction
        {
            GroupId = group.Id,
            PlayerId = null,
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

        return new GuestPixChargeDto(
            transaction.Id,
            request.GroupId,
            request.GuestName,
            chargeId,
            qrCode,
            paymentLink);
    }
}
