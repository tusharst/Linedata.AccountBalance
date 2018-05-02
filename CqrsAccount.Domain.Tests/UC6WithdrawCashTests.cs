namespace CqrsAccount.Domain.Tests
{
    using Xunit;
    using System;
    using System.Threading.Tasks;
    using CqrsAccount.Domain.Tests.Common;
    using ReactiveDomain.Messaging;
    using Xunit.ScenarioReporting;

    [Collection("AggregateTest")]
    public class UC6WithdrawCashTests : IDisposable
    {
        readonly Guid _accountId;
        readonly EventStoreScenarioRunner<Account> _runner;

        public UC6WithdrawCashTests(EventStoreFixture fixture)
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
        public async Task CanWithdrawCashFromValidAccount()
        {
            decimal withdrawAmount = 5000;
            decimal depositeAmount = 5000;

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
            var cmd = new WithdrawCash()
            {
                AccountId = _accountId,
                WithdrawAmount = withdrawAmount
            };

            var ev = new CashWithdrawn(cmd)
            {
                AccountId = _accountId,
                WithdrawAmount = withdrawAmount
            };

            await _runner.Run(
                def => def.Given(accountCreated, evtCashDeposited).When(cmd).Then(ev)
            );
        }

        [Fact]
        public async Task CanWithdrawCashWithOverdraftLimitFromValidAccount()
        {
            decimal depositeAmount = 5000;
            decimal overdraftLimit = 2000;
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

            var cmdConfigureOverdraftLimit = new ConfigureOverdraftLimit
            {
                AccountId = _accountId,
                OverdraftLimit = overdraftLimit
            };

            var evOverdraftLimitConfigured = new OverdraftLimitConfigured(cmdConfigureOverdraftLimit)
            {
                AccountId = cmdConfigureOverdraftLimit.AccountId,
                OverdraftLimit = cmdConfigureOverdraftLimit.OverdraftLimit
            };


            var cmd = new WithdrawCash()
            {
                AccountId = _accountId,
                WithdrawAmount = withdrawAmount
            };

            var ev = new CashWithdrawn(cmd)
            {
                AccountId = _accountId,
                WithdrawAmount = withdrawAmount
            };

            await _runner.Run(
                def => def.Given(accountCreated, evtCashDeposited, evOverdraftLimitConfigured).When(cmd).Then(ev)
            );
        }

        [Fact]
        public async Task CashWithdrawAmountCannotBeNegative()
        {
            decimal withdrawAmount = -5000;

            var accountCreated = new AccountCreated(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                AccountHolderName = "Tushar"
            };

            var cmd = new WithdrawCash()
            {
                AccountId = _accountId,
                WithdrawAmount = withdrawAmount
            };

            await _runner.Run(
                def => def.Given(accountCreated).When(cmd).Throws(new ValidationException("Cash withdrawal amount cannot be negative"))
            );
        }

        [Fact]
        public async Task CashWithdrawShouldThrowExceptionWhenAccountIsNotPresent()
        {
            decimal withdrawAmount = 5000;

            var cmd = new WithdrawCash()
            {
                AccountId = _accountId,
                WithdrawAmount = withdrawAmount
            };

            await _runner.Run(
                def => def.Given().When(cmd).Throws(new ValidationException("No account with this ID exists"))
            );
        }
    }
}
