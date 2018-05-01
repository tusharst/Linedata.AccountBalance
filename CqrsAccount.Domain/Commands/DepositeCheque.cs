namespace CqrsAccount.Domain
{
    using System;
    using ReactiveDomain.Messaging;

    public class DepositCheque : Command
    {
        public DepositCheque()
            : base(NewRoot())
        { }

        public Guid AccountId { get; set; }

        public decimal DepositAmount { get; set; }

        public DateTime DepositDate { get; set; }
    }
}
