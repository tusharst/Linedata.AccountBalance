namespace CqrsAccount.Domain
{
    using System;
    using ReactiveDomain.Messaging;

    public class DepositCash : Command
    {
        public DepositCash()
            : base(NewRoot())
        { }

        public Guid AccountId { get; set; }

        public decimal DepositAmount { get; set; }
        
    }
}
