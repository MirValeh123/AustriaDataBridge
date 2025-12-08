namespace Application.External.Taxograf.Models
{
    public class ScheduledEndpoint
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Method { get; set; } = "GET";
        public TimeSpan Interval { get; set; }
        public DateTime LastRun { get; set; }
        public bool RequiresBody { get; set; }
    }
}
