using System.ComponentModel.DataAnnotations;

namespace WebQuartzApi.Model.Response
{
    public class GetJobsResponse
    {
        [Display(Name = "Scheduler Name")]
        public string SCHED_NAME { get; set; }

        [Display(Name = "Job Name")]
        public string JOB_NAME { get; set; }

        [Display(Name = "Job Group")]
        public string JOB_GROUP { get; set; }

        [Display(Name = "Description")]
        public string DESCRIPTION { get; set; }

        [Display(Name = "Job Class Name")]
        public string JOB_CLASS_NAME { get; set; }

        [Display(Name = "Is Durable")]
        public bool IS_DURABLE { get; set; }

        [Display(Name = "Is Non-Concurrent")]
        public bool IS_NONCONCURRENT { get; set; }

        [Display(Name = "Is Update Data")]
        public bool IS_UPDATE_DATA { get; set; }

        [Display(Name = "Requests Recovery")]
        public bool REQUESTS_RECOVERY { get; set; }

        [Display(Name = "Job Data")]
        public string JOB_DATA { get; set; }
    }
}