namespace Infrastructure.Configuration
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public string RequestLogCollection { get; set; } = "request_logs";
    }
}

