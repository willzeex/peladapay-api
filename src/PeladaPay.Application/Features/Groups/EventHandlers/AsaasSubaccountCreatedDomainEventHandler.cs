using MediatR;
using Microsoft.Extensions.Logging;
using PeladaPay.Application.Events;

namespace PeladaPay.Application.Features.Groups.EventHandlers;

public sealed class AsaasSubaccountCreatedDomainEventHandler(
    ILogger<AsaasSubaccountCreatedDomainEventHandler> logger) : INotificationHandler<AsaasSubaccountCreatedDomainEvent>
{
    public Task Handle(AsaasSubaccountCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Domain event: ASAAS subaccount created. GroupId: {GroupId}, FinancialAccountId: {FinancialAccountId}, ExternalSubaccountId: {ExternalSubaccountId}",
            notification.GroupId,
            notification.FinancialAccountId,
            notification.ExternalSubaccountId);

        return Task.CompletedTask;
    }
}
