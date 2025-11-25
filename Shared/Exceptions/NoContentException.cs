namespace Shared.Exceptions
{
    [Serializable]
    public class NoContentException: ApplicationException
    {
        public NoContentException(string message) : base(message)
        {
        }
    }
}
