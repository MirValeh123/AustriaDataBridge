namespace Shared.Models
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string SmsLogCollection { get; set; }
        public string DLQLogCollection { get; set; }
        public string RequestLogCollection { get; set; }
        public string ExportJobLogCollection { get; set; }
        public string MailLogCollection { get; set; }
        public string FirebaseLogCollection { get; set; }
    }
}
