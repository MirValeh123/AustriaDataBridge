namespace Shared.Exceptions;

public class TokenExpiredException : ApplicationException
{
    public TokenExpiredException()
    {
    }

    public TokenExpiredException(string message)
        : base(message)
    {
    }

    public TokenExpiredException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}