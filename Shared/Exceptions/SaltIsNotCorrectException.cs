namespace Shared.Exceptions
{
    public class SaltIsNotCorrectException : ApplicationException
    {
        public SaltIsNotCorrectException() : base()
        {
        }

        public SaltIsNotCorrectException(string message)
            : base(message)
        {
        }

        public SaltIsNotCorrectException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}