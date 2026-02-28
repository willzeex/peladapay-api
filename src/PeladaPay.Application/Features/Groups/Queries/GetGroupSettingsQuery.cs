using MediatR;
using PeladaPay.Application.DTOs;
using PeladaPay.Application.Interfaces;
using PeladaPay.Domain.Entities;
using PeladaPay.Domain.Exceptions;
using PeladaPay.Domain.Interfaces;

namespace PeladaPay.Application.Features.Groups.Queries;

public sealed record GetGroupSettingsQuery(Guid GroupId) : IRequest<GroupSettingsDto>;

public sealed class GetGroupSettingsQueryHandler(
    IRepository<Group> groupRepository,
    ICurrentUserService currentUserService)
    : IRequestHandler<GetGroupSettingsQuery, GroupSettingsDto>
{
    public async Task<GroupSettingsDto> Handle(GetGroupSettingsQuery request, CancellationToken cancellationToken)
    {
        var organizerId = currentUserService.UserId ?? throw new UnauthorizedAccessException();

        var groups = await groupRepository.GetAsync(
            g => g.Id == request.GroupId && g.OrganizerId == organizerId,
            cancellationToken);

        var group = groups.FirstOrDefault() ?? throw new NotFoundException("Grupo não encontrado para o organizador autenticado.");

        return new GroupSettingsDto(
            group.Id,
            group.Name,
            group.MatchDate,
            group.Frequency ?? string.Empty,
            group.Venue ?? string.Empty,
            group.CrestUrl ?? string.Empty,
            group.FinancialAccountId);
    }
}
