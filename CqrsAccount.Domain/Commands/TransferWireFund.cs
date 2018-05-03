namespace CqrsAccount.Domain
{
    using System;
    using ReactiveDomain.Messaging;

    public class TransferWireFund : Command
    {
        public TransferWireFund()
            : base(NewRoot())
        { }

        public Guid AccountId { get; set; }

        public decimal WireFund { get; set; }

    }
}
