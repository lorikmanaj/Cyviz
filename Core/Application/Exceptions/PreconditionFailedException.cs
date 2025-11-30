namespace Cyviz.Core.Application.Exceptions
{
    public class PreconditionFailedException : Exception
    {
        public PreconditionFailedException(string message) : base(message) { }
    }
}
