namespace WebQuartzApi.Model.Request
{
    public class GetTriggersOfJobRequest
    {
        public string JobName { get; set; }
        public string GroupName { get; set; }
    }
}