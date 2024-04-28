namespace WebQuartzApi.Model.Request
{
    public class StartJobRequest
    {
        public string TriggerName { get; set; }
        public string GroupName { get; set; }
    }
}