namespace CqrsAccount.Domain
{
    using System;
    using Newtonsoft.Json;
    using ReactiveDomain.Messaging;

    /// <summary>
    /// Indicates that a bank account has been created.
    /// </summary>
    public sealed class OverdraftLimitSet : Event
    {
        public OverdraftLimitSet(CorrelatedMessage source)
            : base(source)
        { }

        [JsonConstructor]
        public OverdraftLimitSet(CorrelationId correlationId, SourceId sourceId)
            : base(correlationId, sourceId)
        { }

        public Guid AccountId { get; set; }

        public decimal OverdraftLimit { get; set; }
    }
}
