using MediatR;
using PeladaPay.Application.DTOs;
using PeladaPay.Application.Exceptions;
using PeladaPay.Application.Interfaces;
using PeladaPay.Domain.Entities;
using PeladaPay.Domain.Enums;
using PeladaPay.Domain.Exceptions;
using PeladaPay.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace PeladaPay.Application.Features.Payments.Commands;

public sealed record GenerateGuestPixChargeCommand(Guid GroupId, string? GuestName, string? GuestPhone) : IRequest<GuestPixChargeDto>;

public sealed class GenerateGuestPixChargeCommandHandler(
    IRepository<Group> groupRepository,
    IRepository<FinancialAccount> financialAccountRepository,
    UserManager<ApplicationUser> userManager,
    IRepository<Transaction> transactionRepository,
    IPaymentGatewayStrategy paymentGatewayStrategy,
    IUnitOfWork unitOfWork)
    : IRequestHandler<GenerateGuestPixChargeCommand, GuestPixChargeDto>
{
    private const string DefaultGuestName = "Convidado";
    private const string DefaultCpf = "11144477735";

    /// <summary>
    /// Cria uma cobrança PIX para convidado usando a taxa avulsa configurada no grupo.
    /// </summary>
    public async Task<GuestPixChargeDto> Handle(GenerateGuestPixChargeCommand request, CancellationToken cancellationToken)
    {
        var group = await groupRepository.GetByIdAsync(request.GroupId, cancellationToken)
            ?? throw new NotFoundException("Grupo não encontrado.");

        var financialAccount = await financialAccountRepository.GetByIdAsync(group.FinancialAccountId, cancellationToken)
            ?? throw new NotFoundException("Conta financeira do grupo não encontrada.");

        if (financialAccount.SingleMatchFee <= 0)
            throw new AsaasIntegrationException("Taxa avulsa inválida para gerar cobrança de convidado.");

        var guestName = string.IsNullOrWhiteSpace(request.GuestName) ? DefaultGuestName : request.GuestName.Trim();
        var payerCpf = await ResolvePayerCpfAsync(group.OrganizerId);

        var (chargeId, qrCode, paymentLink) = await paymentGatewayStrategy.CreatePixChargeAsync(
            financialAccount.SingleMatchFee,
            guestName,
            payerCpf,
            null,
            request.GuestPhone,
            cancellationToken);

        var description = $"Pagamento avulso de convidado - {group.Name}";
        var transaction = new Transaction
        {
            GroupId = group.Id,
            PlayerId = null,
            Amount = financialAccount.SingleMatchFee,
            DateUtc = DateTime.UtcNow,
            Type = TransactionType.Entrada,
            Status = TransactionStatus.Pendente,
            ExternalChargeId = chargeId,
            PaymentLink = paymentLink,
            Description = description
        };

        await transactionRepository.AddAsync(transaction, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new GuestPixChargeDto(
            transaction.Id,
            request.GroupId,
            guestName,
            financialAccount.SingleMatchFee,
            chargeId,
            qrCode,
            paymentLink);
    }

    private async Task<string> ResolvePayerCpfAsync(string organizerId)
    {
        var organizer = await userManager.FindByIdAsync(organizerId);
        var sanitizedCpf = SanitizeDigits(organizer?.Cpf);

        return sanitizedCpf.Length == 11 ? sanitizedCpf : DefaultCpf;
    }

    private static string SanitizeDigits(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        return new string(value.Where(char.IsDigit).ToArray());
    }
}
