namespace Shared.Exceptions
{
    [Serializable]
    public class FileServerException : ApplicationException
    {
        public FileServerException() : base("File server exception")
        {
        }

        public FileServerException(string message) : base(message)
        {
        }

        public FileServerException(string message, Exception ex) : base(message, ex)
        {
        }
    }
}