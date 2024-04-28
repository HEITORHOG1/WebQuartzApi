namespace WebQuartzApi.Model.Response
{
    public class GetTriggersOfJobResponse
    {
        public string TriggerName { get; set; }
        public string TriggerGroup { get; set; }
        public string TriggerState { get; set; }
    }
}