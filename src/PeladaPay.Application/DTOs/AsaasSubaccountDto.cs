namespace PeladaPay.Application.DTOs;

public sealed record AsaasSubaccountDto(Guid GroupId, Guid FinancialAccountId, string? ExternalSubaccountId, bool AlreadyExisted);
