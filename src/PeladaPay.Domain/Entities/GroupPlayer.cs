using PeladaPay.Domain.Common;

namespace PeladaPay.Domain.Entities;

public class GroupPlayer : BaseEntity
{
    public Guid GroupId { get; set; }
    public Group? Group { get; set; }

    public Guid PlayerId { get; set; }
    public Player? Player { get; set; }
}
