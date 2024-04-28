namespace WebQuartzApi.Model.Request
{
    public class PauseJobRequest
    {
        public string TriggerName { get; set; }
        public string GroupName { get; set; }
    }
}