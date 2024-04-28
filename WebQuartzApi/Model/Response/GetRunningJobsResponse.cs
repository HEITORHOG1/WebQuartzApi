namespace WebQuartzApi.Model.Response
{
    public class GetRunningJobsResponse
    {
        public string SCHED_NAME { get; set; }
        public string TRIGGER_NAME { get; set; }
        public string TRIGGER_GROUP { get; set; }
        public string JOB_NAME { get; set; }
        public string JOB_GROUP { get; set; }
        public string TRIGGER_STATE { get; set; }
    }
}