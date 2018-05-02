namespace CqrsAccount.Domain
{
    using System;
    using ReactiveDomain.Messaging;

    public class WithdrawCash : Command
    {
        public WithdrawCash()
            : base(NewRoot())
        { }

        public Guid AccountId { get; set; }

        public decimal WithdrawAmount { get; set; }

    }
}
