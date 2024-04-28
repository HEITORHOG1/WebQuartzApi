namespace WebQuartzApi.Model.Request
{
    public class ResumeJobRequest
    {
        public string TriggerName { get; set; }
        public string GroupName { get; set; }
    }
}