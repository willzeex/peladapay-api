using Hangfire;
using PeladaPay.Application.Interfaces.BackgroundJobs;

namespace PeladaPay.Infrastructure.BackgroundJobs;

public sealed class AsaasSubaccountJobScheduler(IBackgroundJobClient backgroundJobClient) : IAsaasSubaccountJobScheduler
{
    public void ScheduleCreateSubaccount(Guid groupId)
    {
        backgroundJobClient.Enqueue<AsaasSubaccountRetryJob>(job => job.ExecuteAsync(groupId, CancellationToken.None));
    }
}
