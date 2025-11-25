using System.Net;

namespace Application.CCC.ExceptionHandling.Models
{
    public class ProblemDetail : BaseProblemDetail
    {
        public ProblemDetail()
        {
            
        }
        public ProblemDetail(Exception exception, HttpStatusCode statusCode, string? exceptionMessage = null)
        {

            Title = "Exception";
            Detail = exceptionMessage ?? exception.Message;
            Status = (int)statusCode;
            TraceId = GenerateTraceId();

        }
    }
}
