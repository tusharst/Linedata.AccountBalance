namespace CqrsAccount.Domain
{
    using System;
    using ReactiveDomain.Messaging;

    public class SetOverdraftLimit : Command
    {
        public SetOverdraftLimit()
            : base(NewRoot())
        { }

        public Guid AccountId { get; set; }

        public decimal OverdraftLimit { get; set; }
    }
}
