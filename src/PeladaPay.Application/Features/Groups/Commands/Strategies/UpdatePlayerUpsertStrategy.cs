using PeladaPay.Domain.Entities;

namespace PeladaPay.Application.Features.Groups.Commands.Strategies;

public sealed class UpdatePlayerUpsertStrategy : IPlayerUpsertStrategy
{
    public bool CanHandle(Player? player) => player is not null;

    public Task<Player> UpsertAsync(Player? player, AddPlayerToGroupCommand request, CancellationToken cancellationToken)
    {
        var existingPlayer = player ?? throw new InvalidOperationException("Jogador não encontrado para atualização.");

        existingPlayer.Name = request.Name;
        existingPlayer.Cpf = request.Cpf.Trim();
        existingPlayer.Email = NormalizeEmail(request.Email);
        existingPlayer.Phone = request.Phone.Trim();
        existingPlayer.Type = request.Type;

        return Task.FromResult(existingPlayer);
    }

    private static string? NormalizeEmail(string? email)
    {
        return string.IsNullOrWhiteSpace(email)
            ? null
            : email.Trim().ToLowerInvariant();
    }
}
