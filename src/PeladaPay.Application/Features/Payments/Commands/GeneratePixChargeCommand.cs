using MediatR;
using PeladaPay.Application.DTOs;
using PeladaPay.Application.Interfaces;
using PeladaPay.Domain.Entities;
using PeladaPay.Domain.Enums;
using PeladaPay.Domain.Interfaces;

namespace PeladaPay.Application.Features.Payments.Commands;

public sealed record GeneratePixChargeCommand(Guid GroupId, decimal Amount, string PlayerName, string Description) : IRequest<PixChargeDto>;

public sealed class GeneratePixChargeCommandHandler(
    IRepository<Transaction> transactionRepository,
    IPaymentGatewayStrategy paymentGatewayStrategy,
    IUnitOfWork unitOfWork) : IRequestHandler<GeneratePixChargeCommand, PixChargeDto>
{
    public async Task<PixChargeDto> Handle(GeneratePixChargeCommand request, CancellationToken cancellationToken)
    {
        var (chargeId, qrCode) = await paymentGatewayStrategy.CreatePixChargeAsync(request.Amount, request.PlayerName, cancellationToken);

        var transaction = new Transaction
        {
            GroupId = request.GroupId,
            Amount = request.Amount,
            DateUtc = DateTime.UtcNow,
            Type = TransactionType.Entrada,
            Status = TransactionStatus.Pendente,
            ExternalChargeId = chargeId,
            Description = request.Description
        };

        await transactionRepository.AddAsync(transaction, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new PixChargeDto(transaction.Id, chargeId, qrCode);
    }
}
