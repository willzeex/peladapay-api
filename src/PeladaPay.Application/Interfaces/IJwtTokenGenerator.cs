using PeladaPay.Domain.Entities;

namespace PeladaPay.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string Generate(ApplicationUser user);
}
