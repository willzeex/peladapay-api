using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PeladaPay.Application.DTOs;
using PeladaPay.Application.Events;
using PeladaPay.Application.Exceptions;
using PeladaPay.Application.Interfaces;
using PeladaPay.Application.Models.Asaas;
using PeladaPay.Domain.Entities;
using PeladaPay.Domain.Exceptions;
using PeladaPay.Domain.Interfaces;

namespace PeladaPay.Application.Features.Groups.Commands;

public sealed record CreateAsaasSubaccountCommand(Guid GroupId) : IRequest<AsaasSubaccountDto>;

public sealed class CreateAsaasSubaccountCommandHandler(
    IRepository<Group> groupRepository,
    IRepository<FinancialAccount> financialAccountRepository,
    UserManager<ApplicationUser> userManager,
    IAsaasService asaasService,
    IUnitOfWork unitOfWork,
    IMediator mediator,
    ILogger<CreateAsaasSubaccountCommandHandler> logger) : IRequestHandler<CreateAsaasSubaccountCommand, AsaasSubaccountDto>
{
    public async Task<AsaasSubaccountDto> Handle(CreateAsaasSubaccountCommand request, CancellationToken cancellationToken)
    {
        var group = await groupRepository.GetByIdAsync(request.GroupId, cancellationToken)
            ?? throw new NotFoundException("Grupo não encontrado.");

        var financialAccount = await financialAccountRepository.GetByIdAsync(group.FinancialAccountId, cancellationToken)
            ?? throw new NotFoundException("Conta financeira não encontrada para o grupo.");

        if (!string.IsNullOrWhiteSpace(financialAccount.ExternalSubaccountId))
        {
            logger.LogInformation("ASAAS subaccount already exists for group {GroupId}.", group.Id);
            return new AsaasSubaccountDto(group.Id, financialAccount.Id, financialAccount.ExternalSubaccountId, true);
        }

        var organizer = await userManager.FindByIdAsync(group.OrganizerId)
            ?? throw new NotFoundException("Organizador não encontrado para criação da subconta ASAAS.");

        if (string.IsNullOrWhiteSpace(organizer.Cpf))
        {
            throw new AsaasIntegrationException("CPF do organizador é obrigatório para criar subconta ASAAS.");
        }

        if (string.IsNullOrWhiteSpace(organizer.Email))
        {
            throw new AsaasIntegrationException("Email do organizador é obrigatório para criar subconta ASAAS.");
        }

        var correlationId = Guid.NewGuid().ToString("N");

        try
        {
            var createRequest = new AsaasCreateAccountRequest(
                organizer.FullName,
                organizer.Email,
                organizer.Cpf,
                organizer.Whatsapp);

            var response = await asaasService.CreateSubaccountAsync(createRequest, cancellationToken);

            financialAccount.ExternalSubaccountId = response.Id;
            financialAccountRepository.Update(financialAccount);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            await mediator.Publish(new AsaasSubaccountCreatedDomainEvent(group.Id, financialAccount.Id, response.Id), cancellationToken);

            logger.LogInformation(
                "ASAAS subaccount created with success. GroupId: {GroupId}, FinancialAccountId: {FinancialAccountId}, CorrelationId: {CorrelationId}",
                group.Id,
                financialAccount.Id,
                correlationId);

            return new AsaasSubaccountDto(group.Id, financialAccount.Id, response.Id, false);
        }
        catch (Exception ex) when (ex is not NotFoundException)
        {
            logger.LogError(
                ex,
                "Failed to create ASAAS subaccount. GroupId: {GroupId}, FinancialAccountId: {FinancialAccountId}, CorrelationId: {CorrelationId}",
                group.Id,
                financialAccount.Id,
                correlationId);

            throw;
        }
    }
}
