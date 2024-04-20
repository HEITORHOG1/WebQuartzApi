namespace WebQuartzApi.Model
{
    /// <summary>
    /// classe que representa um job request
    /// </summary>
    public class JobRequest
    {
        public string JobName { get; set; }
        public string GroupName { get; set; }
        public string CronExpression { get; set; }
        public string Description { get; set; }
        public string SchedulerName { get; set; }
    }
}