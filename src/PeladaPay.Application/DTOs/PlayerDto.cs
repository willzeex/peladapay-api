namespace PeladaPay.Application.DTOs;

public sealed record PlayerDto(Guid Id, string Name, string Cpf, string? Email, string Phone, string Type);
