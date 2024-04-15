namespace WebQuartzApi.Model
{
    /// <summary>
    /// classe que representa um job request
    /// </summary>
    public class JobPausedRequest
    {
        public string JobName { get; set; }
        public string GroupName { get; set; }
    }
}