using PeladaPay.Domain.Common;
using PeladaPay.Domain.Enums;

namespace PeladaPay.Domain.Entities;

public class Player : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public PlayerType Type { get; set; }
    public Guid GroupId { get; set; }
    public Group? Group { get; set; }
}
