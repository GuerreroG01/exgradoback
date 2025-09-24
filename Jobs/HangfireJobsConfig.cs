using Hangfire;
using ExGradoBack.Jobs;

namespace ExGradoBack.Jobs
{
    public static class HangfireJobsConfig
    {
        public static void ConfigurateJobs()
        {
            var zonaHoraria = TimeZoneInfo.FindSystemTimeZoneById("Central America Standard Time");
            var opcionJob = new RecurringJobOptions
            {
                TimeZone = zonaHoraria
            };
            RecurringJob.AddOrUpdate<BackupJob>(
                "backup-job",
                job => job.RunAsync(),
                Cron.Daily(14, 00),
                opcionJob
            );
            RecurringJob.AddOrUpdate<OrdenCompraEmailJob>(
                "orden-compra-job",
                job => job.RunAsync(),
                Cron.Daily(14, 00),
                opcionJob
            );
        }
    }
};