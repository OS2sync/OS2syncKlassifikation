using System;
    
namespace StsKlassifikation.Quartz
{
    public class JobSchedule
    {
        public JobSchedule(Type jobType, string cronExpression, bool enabled, bool runOnStartup)
        {
            JobType = jobType;
            CronExpression = cronExpression;
            Enabled = enabled;
            RunOnStartup = runOnStartup;
        }

        public Type JobType { get; }
        public string CronExpression { get; }
        public bool Enabled { get; set; }

        public bool RunOnStartup { get; set; }
    }
}
