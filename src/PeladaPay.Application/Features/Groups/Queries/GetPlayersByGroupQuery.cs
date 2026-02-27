using MediatR;
using PeladaPay.Application.DTOs;
using PeladaPay.Domain.Entities;
using PeladaPay.Domain.Interfaces;

namespace PeladaPay.Application.Features.Groups.Queries;

public sealed record GetPlayersByGroupQuery(Guid GroupId) : IRequest<IReadOnlyCollection<PlayerDto>>;

public sealed class GetPlayersByGroupQueryHandler(
    IRepository<GroupPlayer> groupPlayerRepository,
    IRepository<Player> playerRepository)
    : IRequestHandler<GetPlayersByGroupQuery, IReadOnlyCollection<PlayerDto>>
{
    public async Task<IReadOnlyCollection<PlayerDto>> Handle(GetPlayersByGroupQuery request, CancellationToken cancellationToken)
    {
        var relations = await groupPlayerRepository.GetAsync(x => x.GroupId == request.GroupId, cancellationToken);
        var playerIds = relations.Select(x => x.PlayerId).ToHashSet();
        var players = await playerRepository.GetAsync(x => playerIds.Contains(x.Id), cancellationToken);

        return players
            .Select(x => new PlayerDto(x.Id, x.Name, x.Cpf, x.Email, x.Phone, x.Type.ToString()))
            .ToList();
    }
}
