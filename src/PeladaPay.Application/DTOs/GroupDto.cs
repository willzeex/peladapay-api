namespace PeladaPay.Application.DTOs;

public sealed record GroupDto(Guid Id, string Name, DateTime MatchDate, Guid FinancialAccountId, string ExternalSubaccountId);
