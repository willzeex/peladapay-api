using MediatR;
using Microsoft.Extensions.Logging;
using PeladaPay.Application.DTOs;
using PeladaPay.Application.Interfaces;
using PeladaPay.Application.Interfaces.BackgroundJobs;
using PeladaPay.Domain.Entities;
using PeladaPay.Domain.Interfaces;

namespace PeladaPay.Application.Features.Groups.Commands;

public sealed record CreateGroupCommand(string Name, DateTime MatchDate, string PixKey) : IRequest<GroupDto>;

public sealed class CreateGroupCommandHandler(
    IRepository<Group> groupRepository,
    IRepository<FinancialAccount> accountRepository,
    ICurrentUserService currentUserService,
    IUnitOfWork unitOfWork,
    IMediator mediator,
    IAsaasSubaccountJobScheduler jobScheduler,
    ILogger<CreateGroupCommandHandler> logger) : IRequestHandler<CreateGroupCommand, GroupDto>
{
    public async Task<GroupDto> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
    {
        var organizerId = currentUserService.UserId ?? throw new UnauthorizedAccessException();

        var account = new FinancialAccount
        {
            Balance = 0,
            PixKey = request.PixKey,
            ExternalSubaccountId = string.Empty
        };

        await accountRepository.AddAsync(account, cancellationToken);

        var group = new Group
        {
            Name = request.Name,
            MatchDate = request.MatchDate,
            FinancialAccountId = account.Id,
            OrganizerId = organizerId
        };

        await groupRepository.AddAsync(group, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        try
        {
            var subaccountResult = await mediator.Send(new CreateAsaasSubaccountCommand(group.Id), cancellationToken);
            return new GroupDto(group.Id, group.Name, group.MatchDate, group.FinancialAccountId, subaccountResult.ExternalSubaccountId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Group created but ASAAS subaccount creation failed for group {GroupId}.", group.Id);
            jobScheduler.ScheduleCreateSubaccount(group.Id);
            return new GroupDto(group.Id, group.Name, group.MatchDate, group.FinancialAccountId, account.ExternalSubaccountId);
        }
    }
}
