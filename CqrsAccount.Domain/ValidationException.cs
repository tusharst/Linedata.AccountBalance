namespace CqrsAccount.Domain
{
    using System;

    public sealed class ValidationException : Exception
    {
        public ValidationException(string message)
            : base(message)
        { }
    }
}
