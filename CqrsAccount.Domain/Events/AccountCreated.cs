namespace CqrsAccount.Domain
{
    using System;
    using Newtonsoft.Json;
    using ReactiveDomain.Messaging;

    /// <summary>
    /// Indicates that a bank account has been created.
    /// </summary>
    public sealed class AccountCreated : Event
    {
        public AccountCreated(CorrelatedMessage source)
            : base(source)
        { }

        [JsonConstructor]
        public AccountCreated(CorrelationId correlationId, SourceId sourceId)
            : base(correlationId, sourceId)
        { }

        public Guid AccountId { get; set; }

        public string AccountHolderName { get; set; }
    }
}
