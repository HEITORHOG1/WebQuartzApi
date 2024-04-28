namespace WebQuartzApi.Model.Request
{
    public class StopJobRequest
    {
        public string TriggerName { get; set; }
        public string GroupName { get; set; }
    }
}