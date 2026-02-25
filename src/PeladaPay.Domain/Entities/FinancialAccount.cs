using PeladaPay.Domain.Common;

namespace PeladaPay.Domain.Entities;

public class FinancialAccount : BaseEntity
{
    public decimal Balance { get; set; }
    public string PixKey { get; set; } = string.Empty;
    public string ExternalSubaccountId { get; set; } = string.Empty;
    public decimal MonthlyFee { get; set; }
    public decimal SingleMatchFee { get; set; }
    public int DueDay { get; set; }
    public bool IsExpenseManagementOnly { get; set; }
}
