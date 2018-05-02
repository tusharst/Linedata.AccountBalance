namespace CqrsAccount.Domain
{
    using System;
    using Newtonsoft.Json;
    using NodaTime;
    using ReactiveDomain.Messaging;

    /// <summary>
    /// Indicates that a bank account has been created.
    /// </summary>
    public sealed class AccountBlocked : Event
    {
        public AccountBlocked(CorrelatedMessage source)
            : base(source)
        { }

        [JsonConstructor]
        public AccountBlocked(CorrelationId correlationId, SourceId sourceId)
            : base(correlationId, sourceId)
        { }

        public Guid AccountId { get; set; }

        public decimal Amount { get; set; }

    }
}
