namespace WebQuartzApi.Model
{
    /// <summary>
    /// classe que representa um job request
    /// </summary>
    public class JobStartedRequest
    {
        public string JobName { get; set; }
        public string GroupName { get; set; }
    }
}