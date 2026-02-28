using MediatR;

namespace PeladaPay.Application.Events;

public sealed record AsaasSubaccountCreatedDomainEvent(Guid GroupId, Guid FinancialAccountId, string ExternalSubaccountId) : INotification;
