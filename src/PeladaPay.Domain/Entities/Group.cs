using PeladaPay.Domain.Common;

namespace PeladaPay.Domain.Entities;

public class Group : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public DateTime MatchDate { get; set; }
    public Guid FinancialAccountId { get; set; }
    public FinancialAccount? FinancialAccount { get; set; }
    public string OrganizerId { get; set; } = string.Empty;
    public ApplicationUser? Organizer { get; set; }
    public ICollection<Player> Players { get; set; } = new List<Player>();
    public ICollection<GroupPlayer> GroupPlayers { get; set; } = new List<GroupPlayer>();
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
