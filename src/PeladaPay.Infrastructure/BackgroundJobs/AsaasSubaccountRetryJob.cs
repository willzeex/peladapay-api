using MediatR;
using Microsoft.Extensions.Logging;
using PeladaPay.Application.Features.Groups.Commands;

namespace PeladaPay.Infrastructure.BackgroundJobs;

public sealed class AsaasSubaccountRetryJob(IMediator mediator, ILogger<AsaasSubaccountRetryJob> logger)
{
    public async Task ExecuteAsync(Guid groupId, CancellationToken cancellationToken)
    {
        try
        {
            await mediator.Send(new CreateAsaasSubaccountCommand(groupId), cancellationToken);
            logger.LogInformation("ASAAS subaccount retry succeeded for group {GroupId}", groupId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "ASAAS subaccount retry failed for group {GroupId}", groupId);
            throw;
        }
    }
}
