namespace PeladaPay.Application.DTOs;

public sealed record ApplicationUserProfileDto(
    string FullName,
    string Email,
    string Whatsapp,
    string Cpf,
    string BirthDate,
    string Address);
