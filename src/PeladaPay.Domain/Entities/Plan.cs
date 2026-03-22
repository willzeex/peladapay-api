using PeladaPay.Domain.Common;

namespace PeladaPay.Domain.Entities;

public class Plan : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public decimal MonthlyPrice { get; set; }
    public decimal PixFeePercentage { get; set; }
    public int FreeWithdrawalsPerMonth { get; set; }
    public decimal AdditionalWithdrawalFee { get; set; }
    public bool IsPopular { get; set; }
    public int DisplayOrder { get; set; }

    public ICollection<PlanFeature> Features { get; set; } = new List<PlanFeature>();
    public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
}
