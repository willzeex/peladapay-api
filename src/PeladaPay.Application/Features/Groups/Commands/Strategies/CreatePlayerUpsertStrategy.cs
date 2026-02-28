using PeladaPay.Domain.Entities;
using PeladaPay.Domain.Interfaces;

namespace PeladaPay.Application.Features.Groups.Commands.Strategies;

public sealed class CreatePlayerUpsertStrategy(IRepository<Player> playerRepository) : IPlayerUpsertStrategy
{
    public bool CanHandle(Player? player) => player is null;

    public async Task<Player> UpsertAsync(Player? player, AddPlayerToGroupCommand request, CancellationToken cancellationToken)
    {
        var newPlayer = new Player
        {
            Name = request.Name,
            Cpf = request.Cpf.Trim(),
            Email = NormalizeEmail(request.Email),
            Phone = request.Phone.Trim(),
            Type = request.Type
        };

        await playerRepository.AddAsync(newPlayer, cancellationToken);
        return newPlayer;
    }

    private static string? NormalizeEmail(string? email)
    {
        return string.IsNullOrWhiteSpace(email)
            ? null
            : email.Trim().ToLowerInvariant();
    }
}
