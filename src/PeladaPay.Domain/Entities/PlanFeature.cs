using PeladaPay.Domain.Common;

namespace PeladaPay.Domain.Entities;

public class PlanFeature : BaseEntity
{
    public Guid PlanId { get; set; }
    public Plan? Plan { get; set; }
    public string Description { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
}
