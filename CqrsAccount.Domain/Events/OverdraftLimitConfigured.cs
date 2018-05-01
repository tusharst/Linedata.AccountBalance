namespace CqrsAccount.Domain
{
    using System;
    using Newtonsoft.Json;
    using ReactiveDomain.Messaging;

    /// <summary>
    /// Indicates that a bank account has been created.
    /// </summary>
    public sealed class OverdraftLimitConfigured : Event
    {
        public OverdraftLimitConfigured(CorrelatedMessage source)
            : base(source)
        { }

        [JsonConstructor]
        public OverdraftLimitConfigured(CorrelationId correlationId, SourceId sourceId)
            : base(correlationId, sourceId)
        { }

        public Guid AccountId { get; set; }

        public decimal OverdraftLimit { get; set; }
    }
}
