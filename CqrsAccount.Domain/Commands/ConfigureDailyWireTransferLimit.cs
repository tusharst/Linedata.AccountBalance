namespace CqrsAccount.Domain
{
    using System;
    using ReactiveDomain.Messaging;

    public class ConfigureDailyWireTransferLimit : Command
    {
        public ConfigureDailyWireTransferLimit()
            : base(NewRoot())
        { }

        public Guid AccountId { get; set; }

        public decimal DailyWireTransferLimit { get; set; }
    }
}
