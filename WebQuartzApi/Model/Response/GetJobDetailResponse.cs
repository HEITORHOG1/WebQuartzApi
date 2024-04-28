namespace WebQuartzApi.Model.Response
{
    public class GetJobDetailResponse
    {
        public string SCHED_NAME { get; set; }
        public string JOB_NAME { get; set; }
        public string JOB_GROUP { get; set; }
        public string DESCRIPTION { get; set; }
        public string JOB_CLASS_NAME { get; set; }
        public bool IS_DURABLE { get; set; }
        public bool IS_NONCONCURRENT { get; set; }
        public bool IS_UPDATE_DATA { get; set; }
        public bool REQUESTS_RECOVERY { get; set; }
        public string JOB_DATA { get; set; }
    }
}