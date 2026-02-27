using PeladaPay.Domain.Entities;

namespace PeladaPay.Application.Features.Groups.Commands.Strategies;

public interface IPlayerUpsertStrategy
{
    bool CanHandle(Player? player);
    Task<Player> UpsertAsync(Player? player, AddPlayerToGroupCommand request, CancellationToken cancellationToken);
}
