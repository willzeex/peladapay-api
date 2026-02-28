namespace PeladaPay.Application.DTOs;

public sealed record GroupSettingsDto(
    Guid Id,
    string Name,
    DateTime MatchDate,
    string Frequency,
    string Venue,
    string CrestUrl,
    Guid FinancialAccountId);
