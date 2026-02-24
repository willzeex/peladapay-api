using PeladaPay.Domain.Common;
using PeladaPay.Domain.Enums;

namespace PeladaPay.Domain.Entities;

public class Player : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public PlayerType Type { get; set; }
    public ICollection<Group> Groups { get; set; } = new List<Group>();
    public ICollection<GroupPlayer> GroupPlayers { get; set; } = new List<GroupPlayer>();
}
