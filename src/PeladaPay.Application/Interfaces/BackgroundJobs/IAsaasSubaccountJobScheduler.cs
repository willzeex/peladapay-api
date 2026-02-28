namespace PeladaPay.Application.Interfaces.BackgroundJobs;

public interface IAsaasSubaccountJobScheduler
{
    void ScheduleCreateSubaccount(Guid groupId);
}
