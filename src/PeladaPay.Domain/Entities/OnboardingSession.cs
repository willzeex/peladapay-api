using PeladaPay.Domain.Common;

namespace PeladaPay.Domain.Entities;

public class OnboardingSession : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Whatsapp { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public string? Cpf { get; set; }
    public DateOnly? BirthDate { get; set; }
    public string? Address { get; set; }

    public string? GroupName { get; set; }
    public string? Frequency { get; set; }
    public string? Venue { get; set; }
    public string? CrestUrl { get; set; }

    public int CurrentStep { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
}
