using MediatR;
using PeladaPay.Application.DTOs;
using PeladaPay.Application.Features.Groups.Commands.Strategies;
using PeladaPay.Application.Interfaces;
using PeladaPay.Domain.Entities;
using PeladaPay.Domain.Enums;
using PeladaPay.Domain.Interfaces;

namespace PeladaPay.Application.Features.Groups.Commands;

public sealed record AddPlayerToGroupCommand(
    Guid GroupId,
    string Name,
    string Cpf,
    string? Email,
    string Phone,
    PlayerType Type) : IRequest<PlayerDto>;

public sealed class AddPlayerToGroupCommandHandler(
    IRepository<Group> groupRepository,
    IRepository<Player> playerRepository,
    IRepository<GroupPlayer> groupPlayerRepository,
    IEnumerable<IPlayerUpsertStrategy> playerUpsertStrategies,
    ICurrentUserService currentUserService,
    IUnitOfWork unitOfWork) : IRequestHandler<AddPlayerToGroupCommand, PlayerDto>
{
    public async Task<PlayerDto> Handle(AddPlayerToGroupCommand request, CancellationToken cancellationToken)
    {
        var organizerId = currentUserService.UserId ?? throw new UnauthorizedAccessException();

        var group = await groupRepository.GetByIdAsync(request.GroupId, cancellationToken)
            ?? throw new InvalidOperationException("Grupo não encontrado.");

        if (group.OrganizerId != organizerId)
        {
            throw new UnauthorizedAccessException("Acesso negado ao grupo.");
        }

        var cpf = request.Cpf.Trim();
        var existingPlayer = (await playerRepository.GetAsync(x => x.Cpf == cpf, cancellationToken)).FirstOrDefault();

        var upsertStrategy = playerUpsertStrategies.FirstOrDefault(x => x.CanHandle(existingPlayer))
            ?? throw new InvalidOperationException("Estratégia de cadastro de jogador não encontrada.");

        var player = await upsertStrategy.UpsertAsync(existingPlayer, request with { Cpf = cpf }, cancellationToken);

        var alreadyInGroup = (await groupPlayerRepository
            .GetAsync(x => x.GroupId == request.GroupId && x.PlayerId == player.Id, cancellationToken))
            .Any();

        if (!alreadyInGroup)
        {
            await groupPlayerRepository.AddAsync(new GroupPlayer
            {
                GroupId = request.GroupId,
                PlayerId = player.Id
            }, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new PlayerDto(player.Id, player.Name, player.Cpf, player.Email, player.Phone, player.Type.ToString());
    }
}
