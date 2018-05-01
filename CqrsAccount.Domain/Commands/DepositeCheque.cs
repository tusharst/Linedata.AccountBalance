namespace CqrsAccount.Domain
{
    using System;
    using ReactiveDomain.Messaging;

    public class DepositeCheque : Command
    {
        public DepositeCheque()
            : base(NewRoot())
        { }

        public Guid AccountId { get; set; }

        public decimal DepositeAmount { get; set; }

        public DateTime DepositeDate { get; set; }
    }
}
