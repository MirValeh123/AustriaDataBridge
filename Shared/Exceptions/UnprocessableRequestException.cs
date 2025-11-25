

using FluentValidation.Results;

namespace Shared.Exceptions
{
    [Serializable]
    public class UnprocessableRequestException : ApplicationException
    {
        public UnprocessableRequestException()
            : base("One or more validation failures have occurred.")
        {
            Errors = new Dictionary<string, string[]>();
        }

        public UnprocessableRequestException(IEnumerable<ValidationFailure> failures)
            : this()
        {
            Errors = failures
                .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
        }
        public IDictionary<string, string[]>? Errors { get; }
    }
}
