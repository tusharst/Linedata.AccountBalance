namespace CqrsAccount.Domain
{
    using System;
    using ReactiveDomain;
    using ReactiveDomain.Messaging;

    public sealed class Account : EventDrivenStateMachine
    {
        Account()
        {
            Register<AccountCreated>(
                e => { Id = e.AccountId; }
            );
            Register<OverdraftLimitSet>(e => { });
        }

        public static Account Create(Guid id, string accountHolderName, CorrelatedMessage source)
        {
            if (string.IsNullOrWhiteSpace(accountHolderName))
                throw new ValidationException("A valid account owner name must be provided");

            var account = new Account();

            account.Raise(new AccountCreated(source)
            {
                AccountId = id,
                AccountHolderName = accountHolderName
            });

            return account;
        }

        public void SetOverdraftLimit(decimal overdraftLimit, CorrelatedMessage source)
        {
            if(overdraftLimit < 0)
                throw new ValidationException("Overdraft limit must be positive");

            Raise(new OverdraftLimitSet(source)
            {
                AccountId = Id,
                OverdraftLimit = overdraftLimit
            });
        }
    }
}
