using System.Net;
using Shared.Exceptions;

namespace Application.CCC.ExceptionHandling.Models
{
    public class ValidationProblemDetail : BaseProblemDetail
    {
        public ValidationProblemDetail()
        {
            
        }
        public IDictionary<string, string[]> Errors;
        public ValidationProblemDetail(Exception exception)
        {
            Title = "Validation Exception";
            Detail = "One or more validation exception occured.";
            Status = (int)HttpStatusCode.UnprocessableEntity;
            TraceId = GenerateTraceId();
            Errors = (exception as UnprocessableRequestException).Errors;
        }
    }
}
