namespace PeladaPay.Application.DTOs;

public sealed record UserProfileDto(
    string FullName,
    string Email,
    string? Whatsapp,
    string? Cpf,
    DateOnly? BirthDate,
    string? Address);
