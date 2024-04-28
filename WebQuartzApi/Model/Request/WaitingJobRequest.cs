namespace WebQuartzApi.Model.Request
{
    public class WaitingJobRequest
    {
        public string TriggerName { get; set; }
        public string GroupName { get; set; }
    }
}