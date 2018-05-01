namespace CqrsAccount.Domain
{
    using System;
    using Newtonsoft.Json;
    using ReactiveDomain.Messaging;

    /// <summary>
    /// Indicates that a bank account has been created.
    /// </summary>
    public sealed class ChequeDeposited : Event
    {
        public ChequeDeposited(CorrelatedMessage source)
            : base(source)
        { }

        [JsonConstructor]
        public ChequeDeposited(CorrelationId correlationId, SourceId sourceId)
            : base(correlationId, sourceId)
        { }

        public Guid AccountId { get; set; }

        public decimal DepositAmount { get; set; }

        public DateTime DepositDate { get; set; }
    }
}
