namespace CqrsAccount.Domain.Tests
{
    using Xunit;
    using System;
    using System.Threading.Tasks;
    using CqrsAccount.Domain.Tests.Common;
    using ReactiveDomain.Messaging;
    using Xunit.ScenarioReporting;

    [Collection("AggregateTest")]
    public class UC9UnblockAccountTest : IDisposable
    {
        readonly Guid _accountId;
        readonly EventStoreScenarioRunner<Account> _runner;

        public UC9UnblockAccountTest(EventStoreFixture fixture)
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
        public async Task ShouldUnblockOnCashDepositWhenAccountIsAlreadyBlocked()
        {
            decimal depositeAmount = 5000;
            decimal overdraftLimit = 1000;
            decimal withdrawAmount = 7000;

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

            var evOverdraftLimitConfigured = new OverdraftLimitConfigured(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                OverdraftLimit = overdraftLimit
            };

            var evtCashWithdrawn = new CashWithdrawn(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                WithdrawAmount = withdrawAmount
            };

            var evAccountBlocked = new AccountBlocked(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                Amount = withdrawAmount
            };

            var cmd = new DepositCash()
            {
                AccountId = _accountId,
                DepositAmount = depositeAmount
            };

            var evAccountUnblocked = new AccountUnblocked(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                Amount = depositeAmount
            };

            // Yet to decide how to tackle this.
            //await _runner.Run(
            //    def => def.Given(accountCreated, evtCashDeposited, evOverdraftLimitConfigured, evtCashWithdrawn, evAccountBlocked).When(cmd).Then(evAccountUnblocked)
            //);
        }

       

    }
}
