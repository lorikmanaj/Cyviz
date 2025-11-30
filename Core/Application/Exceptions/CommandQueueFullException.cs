namespace Cyviz.Core.Application.Exceptions
{
    public class CommandQueueFullException : Exception
    {
        public CommandQueueFullException()
            : base("Command queue is full. Please retry later.")
        {
        }
    }
}
