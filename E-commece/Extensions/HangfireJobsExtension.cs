using E_commerce.BLL.Services.Interfaces;
using Hangfire;

public static class HangfireJobsExtension
{
    public static WebApplication RegisterRecurringJobs(this WebApplication app)
    {
        RecurringJob.AddOrUpdate<IDailyJobService>(
            "cleanup-unpaid-orders",
            x => x.CleanUpUnpaidOrdersOlderThanOneMonthAsync(),
            Cron.Daily);

        return app;
    }
}