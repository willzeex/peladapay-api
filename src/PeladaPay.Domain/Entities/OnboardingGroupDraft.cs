using PeladaPay.Domain.Common;
using PeladaPay.Domain.Enums;

namespace PeladaPay.Domain.Entities;

public class OnboardingGroupDraft : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }
    public string? Name { get; set; }
    public GroupFrequency? Frequency { get; set; }
    public string? Venue { get; set; }
    public string? CrestUrl { get; set; }
}
