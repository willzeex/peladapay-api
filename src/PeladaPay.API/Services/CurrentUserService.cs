using System.Security.Claims;
using PeladaPay.Application.Interfaces;

namespace PeladaPay.API.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public string? UserId => httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
}
