using Newtonsoft.Json;

namespace Application.CCC.ExceptionHandling.Models
{
    public class BaseProblemDetail
    {
        public string? TraceId { get; set; }
        public string? Title { get; set; }
        public int? Status { get; set; }
        public string? Detail { get; set; }

        public virtual string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        protected string GenerateTraceId()
            => $"{DateTime.Now:dd-MM-yyyy:HH:mm:ss}-{Guid.NewGuid().ToString()}";
    }

}
