namespace CqrsAccount.Domain
{
    using System;
    using NodaTime;
    using ReactiveDomain;
    using ReactiveDomain.Messaging;

    public sealed class Account : EventDrivenStateMachine
    {
        decimal _accountBalance;
        decimal _accountOverdraftLimit;
        decimal _accountDailyWireTransferLimit;
        bool _isBlocked;

        Account()
        {
            Register<AccountCreated>( e => { Id = e.AccountId; } );
            Register<OverdraftLimitConfigured>(e => { _accountOverdraftLimit = e.OverdraftLimit;  });
            Register<DailyWireTransferLimitConfigured>(e => { _accountDailyWireTransferLimit = e.DailyWireTransferLimit;  });
            Register<ChequeDeposited>(e => { });
            Register<CashDeposited>(e => { _accountBalance += e.DepositAmount; } );
            Register<CashWithdrawn>(e => { _accountBalance -= e.WithdrawAmount; });
            Register<AccountBlocked>(e => { _isBlocked = true; });
            Register<AccountUnblocked>(e => { _isBlocked = false; });
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

            if (_isBlocked)
            {
                Raise(new AccountUnblocked(source)
                {
                    AccountId = Id,
                    Amount = depositeAmount
                });
            }
        }

        public void WithdrawCashFromAccount(decimal withdrawAmount, CorrelatedMessage source)
        {
            if (withdrawAmount < 0)
                throw new ValidationException("Cash withdrawal amount cannot be negative");

            if (withdrawAmount > (_accountBalance + _accountOverdraftLimit))
            {
                Raise(new AccountBlocked(source)
                {
                    AccountId = Id,
                    Amount = withdrawAmount
                });
                return;
            }

            Raise(new CashWithdrawn(source)
            {
                AccountId = Id,
                WithdrawAmount = withdrawAmount
            });
        }

        // In progress.......
        //public void TransferWireFund(decimal withdrawAmount, IClock clock, CorrelatedMessage source)
        //{
        //    if (withdrawAmount < 0)
        //        throw new ValidationException("Transfer wire fund cannot be negative");

        //    if (withdrawAmount > (_accountBalance + _accountOverdraftLimit))
        //    {
        //        Raise(new AccountBlocked(source)
        //        {
        //            AccountId = Id,
        //            Amount = withdrawAmount
        //        });
        //        return;
        //    }

        //}
    }
}
