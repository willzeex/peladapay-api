using MediatR;
using PeladaPay.Application.DTOs;
using PeladaPay.Domain.Entities;
using PeladaPay.Domain.Interfaces;

namespace PeladaPay.Application.Features.Groups.Queries;

public sealed record GetPlayerActivitiesQuery(Guid PlayerId) : IRequest<IReadOnlyCollection<PlayerActivityDto>>;

public sealed class GetPlayerActivitiesQueryHandler(
    IRepository<GroupPlayer> groupPlayerRepository,
    IRepository<Group> groupRepository)
    : IRequestHandler<GetPlayerActivitiesQuery, IReadOnlyCollection<PlayerActivityDto>>
{
    public async Task<IReadOnlyCollection<PlayerActivityDto>> Handle(GetPlayerActivitiesQuery request, CancellationToken cancellationToken)
    {
        var relations = await groupPlayerRepository.GetAsync(x => x.PlayerId == request.PlayerId, cancellationToken);
        var groupIds = relations.Select(x => x.GroupId).ToHashSet();
        var groups = await groupRepository.GetAsync(x => groupIds.Contains(x.Id), cancellationToken);

        return groups
            .Select(x => new PlayerActivityDto(x.Id, x.Name, x.MatchDate))
            .OrderBy(x => x.MatchDate)
            .ToList();
    }
}
