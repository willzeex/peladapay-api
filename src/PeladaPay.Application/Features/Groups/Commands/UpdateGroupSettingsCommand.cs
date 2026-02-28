using MediatR;
using PeladaPay.Application.DTOs;
using PeladaPay.Application.Interfaces;
using PeladaPay.Domain.Entities;
using PeladaPay.Domain.Exceptions;
using PeladaPay.Domain.Interfaces;

namespace PeladaPay.Application.Features.Groups.Commands;

public sealed record UpdateGroupSettingsCommand(
    Guid GroupId,
    string? Name,
    DateTime? MatchDate,
    string? Frequency,
    string? Venue,
    string? CrestUrl) : IRequest<GroupDto>;

public sealed class UpdateGroupSettingsCommandHandler(
    IRepository<Group> groupRepository,
    IRepository<FinancialAccount> financialAccountRepository,
    ICurrentUserService currentUserService,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateGroupSettingsCommand, GroupDto>
{
    public async Task<GroupDto> Handle(UpdateGroupSettingsCommand request, CancellationToken cancellationToken)
    {
        var organizerId = currentUserService.UserId ?? throw new UnauthorizedAccessException();

        var groups = await groupRepository.GetAsync(g => g.Id == request.GroupId && g.OrganizerId == organizerId, cancellationToken);
        var group = groups.FirstOrDefault() ?? throw new NotFoundException("Grupo não encontrado para o organizador autenticado.");

        var hasChanges = false;

        if (request.Name is not null)
        {
            group.Name = request.Name.Trim();
            hasChanges = true;
        }

        if (request.MatchDate.HasValue)
        {
            group.MatchDate = request.MatchDate.Value;
            hasChanges = true;
        }

        if (request.Frequency is not null)
        {
            group.Frequency = string.IsNullOrWhiteSpace(request.Frequency)
                ? null
                : request.Frequency.Trim();
            hasChanges = true;
        }

        if (request.Venue is not null)
        {
            group.Venue = string.IsNullOrWhiteSpace(request.Venue)
                ? null
                : request.Venue.Trim();
            hasChanges = true;
        }

        if (request.CrestUrl is not null)
        {
            group.CrestUrl = string.IsNullOrWhiteSpace(request.CrestUrl)
                ? null
                : request.CrestUrl.Trim();
            hasChanges = true;
        }

        if (hasChanges)
        {
            groupRepository.Update(group);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        var financialAccount = await financialAccountRepository.GetByIdAsync(group.FinancialAccountId, cancellationToken)
            ?? throw new NotFoundException("Conta financeira do grupo não encontrada.");

        return new GroupDto(group.Id, group.Name, group.MatchDate, group.FinancialAccountId, financialAccount.ExternalSubaccountId);
    }
}
