using MediatR;
using PeladaPay.Application.DTOs;
using PeladaPay.Application.Interfaces;
using PeladaPay.Domain.Entities;
using PeladaPay.Domain.Interfaces;

namespace PeladaPay.Application.Features.Groups.Commands;

public sealed record CreateGroupCommand(string Name, DateTime MatchDate, string PixKey) : IRequest<GroupDto>;

public sealed class CreateGroupCommandHandler(
    IRepository<Group> groupRepository,
    IRepository<FinancialAccount> accountRepository,
    ICurrentUserService currentUserService,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateGroupCommand, GroupDto>
{
    public async Task<GroupDto> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
    {
        var managerId = currentUserService.UserId ?? throw new UnauthorizedAccessException();

        var account = new FinancialAccount
        {
            Balance = 0,
            PixKey = request.PixKey,
            ExternalSubaccountId = $"subacc_{Guid.NewGuid():N}" // mock BaaS
        };

        await accountRepository.AddAsync(account, cancellationToken);

        var group = new Group
        {
            Name = request.Name,
            MatchDate = request.MatchDate,
            FinancialAccountId = account.Id,
            ManagerId = managerId
        };

        await groupRepository.AddAsync(group, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new GroupDto(group.Id, group.Name, group.MatchDate, group.FinancialAccountId, account.ExternalSubaccountId);
    }
}
