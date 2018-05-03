namespace CqrsAccount.Domain
{
    using System;
    using Newtonsoft.Json;
    using ReactiveDomain.Messaging;

    /// <summary>
    /// Indicates that a bank account has been created.
    /// </summary>
    public sealed class WireFundTransferred : Event
    {
        public WireFundTransferred(CorrelatedMessage source)
            : base(source)
        { }

        [JsonConstructor]
        public WireFundTransferred(CorrelationId correlationId, SourceId sourceId)
            : base(correlationId, sourceId)
        { }

        public Guid AccountId { get; set; }

        public decimal WireFund { get; set; }

    }
}
