namespace PeladaPay.Application.DTOs;

public sealed record PlayerDto(Guid Id, string Name, string Email, string Phone, string Type);
