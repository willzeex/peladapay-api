using PeladaPay.Domain.Common;
using PeladaPay.Domain.Enums;

namespace PeladaPay.Domain.Entities;

public class Transaction : BaseEntity
{
    public Guid GroupId { get; set; }
    public Group? Group { get; set; }
    public Guid? PlayerId { get; set; }
    public Player? Player { get; set; }
    public decimal Amount { get; set; }
    public DateTime DateUtc { get; set; }
    public TransactionType Type { get; set; }
    public TransactionStatus Status { get; set; }
    public string? ReceiptUrl { get; set; }
    public string? ExternalChargeId { get; set; }
    public string? PaymentLink { get; set; }
    public string Description { get; set; } = string.Empty;
}
