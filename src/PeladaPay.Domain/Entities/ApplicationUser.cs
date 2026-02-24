using Microsoft.AspNetCore.Identity;

namespace PeladaPay.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public ICollection<Group> OrganizedGroups { get; set; } = new List<Group>();
}
