namespace CqrsAccount.Domain
{
    using System;
    using ReactiveDomain;
    using ReactiveDomain.Messaging;

    public sealed class Account : EventDrivenStateMachine
    {
        decimal accountBalance;

        Account()
        {
            Register<AccountCreated>(
                e => { Id = e.AccountId; }
            );
            Register<OverdraftLimitConfigured>(e => { });
            Register<DailyWireTransferLimitConfigured>(e => { });
            Register<ChequeDeposited>(e => { });
            Register<CashDeposited>(e => { accountBalance += e.DepositAmount;  });
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

        public void ConfigureOverdraftLimit(decimal overdraftLimit, CorrelatedMessage source)
        {
            if(overdraftLimit < 0)
                throw new ValidationException("Overdraft limit must be positive");

            Raise(new OverdraftLimitConfigured(source)
            {
                AccountId = Id,
                OverdraftLimit = overdraftLimit
            });
        }

        public void ConfigureDailyWireTransferLimit(decimal dailyWireTransferLimit, CorrelatedMessage source)
        {
            if (dailyWireTransferLimit < 0)
                throw new ValidationException("Daily wire transfer limit cannot be negative");

            Raise(new DailyWireTransferLimitConfigured(source)
            {
                AccountId = Id,
                DailyWireTransferLimit = dailyWireTransferLimit
            });
        }

        public void DepositChequeIntoAccount(decimal depositAmount,DateTime depositeDate, CorrelatedMessage source)
        {
            if (depositAmount < 0)
                throw new ValidationException("Cheque deposit amount cannot be negative");

            Raise(new ChequeDeposited(source)
            {
                AccountId = Id,
                DepositAmount = depositAmount,
                DepositDate = depositeDate
            });
        }

        public void DepositCashIntoAccount(decimal depositeAmount, CorrelatedMessage source)
        {
            if (depositeAmount < 0)
                throw new ValidationException("Cash deposit amount cannot be negative");

            Raise(new CashDeposited(source)
            {
                AccountId = Id,
                DepositAmount = depositeAmount
            });
        }
    }
}
