namespace CqrsAccount.Domain
{
    using System;
    using ReactiveDomain.Messaging;

    public class ConfigureOverdraftLimit : Command
    {
        public ConfigureOverdraftLimit()
            : base(NewRoot())
        { }

        public Guid AccountId { get; set; }

        public decimal OverdraftLimit { get; set; }
    }
}
