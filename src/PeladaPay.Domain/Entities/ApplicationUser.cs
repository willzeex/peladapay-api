using Microsoft.AspNetCore.Identity;

namespace PeladaPay.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public string? Whatsapp { get; set; }
    public string? Cpf { get; set; }
    public DateOnly? BirthDate { get; set; }
    public string? Address { get; set; }

    public string? OnboardingGroupName { get; set; }
    public string? OnboardingFrequency { get; set; }
    public string? OnboardingVenue { get; set; }
    public string? OnboardingCrestUrl { get; set; }
    public int OnboardingCurrentStep { get; set; }
    public DateTime? OnboardingCompletedAtUtc { get; set; }

    public ICollection<Group> OrganizedGroups { get; set; } = new List<Group>();
}
