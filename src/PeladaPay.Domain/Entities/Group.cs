using PeladaPay.Domain.Common;

namespace PeladaPay.Domain.Entities;

public class Group : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public DateTime MatchDate { get; set; }
    public Guid FinancialAccountId { get; set; }
    public FinancialAccount? FinancialAccount { get; set; }
    public string ManagerId { get; set; } = string.Empty;
    public ApplicationUser? Manager { get; set; }
    public ICollection<Player> Players { get; set; } = new List<Player>();
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
