using MongoDB.Bson.Serialization.Attributes;

namespace Application.Models.Response
{
    public class BaseLog
    {
        public BaseLog()
        {
            LogDate = DateTime.Now;
        }

        [BsonId]
        public Guid Id { get; set; }
        public DateTime? LogDate { get; set; }
        public string RequestUrl { get; set; }
        public string RequestHeaders { get; set; }
        public int ResponseStatusCode { get; set; }
        public string ResponseBody { get; set; }
        public string ResponseHeaders { get; set; }
        public DateTime RequestDate { get; set; } = DateTime.UtcNow;
        public string? RequestIP { get; set; }
        public string RequestDuration { get; set; }
    }
}
