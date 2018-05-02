namespace CqrsAccount.Domain
{
    using System;
    using Newtonsoft.Json;
    using ReactiveDomain.Messaging;

    /// <summary>
    /// Indicates that a bank account has been created.
    /// </summary>
    public sealed class CashWithdrawn : Event
    {
        public CashWithdrawn(CorrelatedMessage source)
            : base(source)
        { }

        [JsonConstructor]
        public CashWithdrawn(CorrelationId correlationId, SourceId sourceId)
            : base(correlationId, sourceId)
        { }

        public Guid AccountId { get; set; }

        public decimal WithdrawAmount { get; set; }

    }
}
