namespace CqrsAccount.Domain.Tests
{
    using Xunit;
    using System;
    using System.Threading.Tasks;
    using CqrsAccount.Domain.Tests.Common;
    using ReactiveDomain.Messaging;
    using Xunit.ScenarioReporting;

    [Collection("AggregateTest")]
    public class US8WithdrawalBlocksAccount : IDisposable
    {
        readonly Guid _accountId;
        readonly EventStoreScenarioRunner<Account> _runner;

        public US8WithdrawalBlocksAccount(EventStoreFixture fixture)
        {
            _accountId = Guid.NewGuid();
            _runner = new EventStoreScenarioRunner<Account>(
                _accountId,
                fixture,
                (repository, dispatcher) => new AccountCommandHandler(repository, dispatcher));
        }

        public void Dispose()
        {
            _runner.Dispose();
        }

        [Fact]
        public async Task CannotWithdrawCashOutsideBalanceAndWithOverdraftLimit()
        {
            decimal depositeAmount = 5000;
            decimal overdraftLimit = 1000;
            decimal withdrawAmount = 7000;

            var accountCreated = new AccountCreated(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                AccountHolderName = "Tushar"
            };
            var cmdDepositCash = new DepositCash
            {
                AccountId = _accountId,
                DepositAmount = depositeAmount
            };

            var evtCashDeposited = new CashDeposited(cmdDepositCash)
            {
                AccountId = _accountId,
                DepositAmount = depositeAmount
            };

            var evOverdraftLimitConfigured = new OverdraftLimitConfigured(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                OverdraftLimit = overdraftLimit
            };

            var cmd = new WithdrawCash()
            {
                AccountId = _accountId,
                WithdrawAmount = withdrawAmount
            };

            var evAccountBlocked = new AccountBlocked(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                Amount = withdrawAmount
            };

            await _runner.Run(
                def => def.Given(accountCreated, evtCashDeposited, evOverdraftLimitConfigured).When(cmd).Then(evAccountBlocked)
            );
        }


        [Fact]
        public async Task CashWithdrawalGreaterThanAllowedLimit()
        {
            decimal withdrawAmount = 10000;
            decimal depositeAmount = 5000;

            var accountCreated = new AccountCreated(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                AccountHolderName = "Tushar"
            };

            var evtCashDeposited = new CashDeposited(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                DepositAmount = depositeAmount
            };

            var cmdWithdrawCash = new WithdrawCash()
            {
                AccountId = _accountId,
                WithdrawAmount = withdrawAmount
            };

            var evAccountBlocked = new AccountBlocked(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                Amount = withdrawAmount
            };

            await _runner.Run(
                def => def.Given(accountCreated, evtCashDeposited).When(cmdWithdrawCash).Then(evAccountBlocked)
            );
        }

    }
}
