namespace CqrsAccount.Domain
{
    using System;
    using Newtonsoft.Json;
    using ReactiveDomain.Messaging;

    /// <summary>
    /// Indicates that a bank account has been created.
    /// </summary>
    public sealed class DailyWireTransferLimitConfigured : Event
    {
        public DailyWireTransferLimitConfigured(CorrelatedMessage source)
            : base(source)
        { }

        [JsonConstructor]
        public DailyWireTransferLimitConfigured(CorrelationId correlationId, SourceId sourceId)
            : base(correlationId, sourceId)
        { }

        public Guid AccountId { get; set; }

        public decimal DailyWireTransferLimit { get; set; }
    }
}
