using MediatR;
using PeladaPay.Application.DTOs;
using PeladaPay.Domain.Entities;
using PeladaPay.Domain.Interfaces;

namespace PeladaPay.Application.Features.Groups.Queries;

public sealed record GetGroupsByOrganizerQuery(string OrganizerId) : IRequest<IReadOnlyCollection<GroupDto>>;

public sealed class GetGroupsByOrganizerQueryHandler(
    IRepository<Group> groupRepository,
    IRepository<FinancialAccount> accountRepository)
    : IRequestHandler<GetGroupsByOrganizerQuery, IReadOnlyCollection<GroupDto>>
{
    public async Task<IReadOnlyCollection<GroupDto>> Handle(GetGroupsByOrganizerQuery request, CancellationToken cancellationToken)
    {
        var groups = await groupRepository.GetAsync(x => x.OrganizerId == request.OrganizerId, cancellationToken);
        var accounts = await accountRepository.GetAsync(x => groups.Select(g => g.FinancialAccountId).Contains(x.Id), cancellationToken);

        return groups
            .OrderBy(x => x.MatchDate)
            .Select(group =>
            {
                var account = accounts.First(x => x.Id == group.FinancialAccountId);
                return new GroupDto(group.Id, group.Name, group.MatchDate, group.FinancialAccountId, account.ExternalSubaccountId);
            })
            .ToList();
    }
}
