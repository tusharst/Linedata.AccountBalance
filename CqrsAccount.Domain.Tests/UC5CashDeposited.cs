namespace CqrsAccount.Domain.Tests
{
    using Xunit;
    using System;
    using System.Threading.Tasks;
    using CqrsAccount.Domain.Tests.Common;
    using ReactiveDomain.Messaging;
    using Xunit.ScenarioReporting;

    [Collection("AggregateTest")]
    public class UC5CashDeposited : IDisposable
    {
        readonly Guid _accountId;
        readonly EventStoreScenarioRunner<Account> _runner;

        public UC5CashDeposited(EventStoreFixture fixture)
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
        public async Task CanDepositCashInToValidAccount()
        {
            decimal depositeAmount = 5000;
            DateTime depositeDate = System.DateTime.Now;
            var accountCreated = new AccountCreated(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                AccountHolderName = "Tushar"
            };
            var cmd = new DepositCash
            {
                AccountId = _accountId,
                DepositAmount = depositeAmount                
            };

            var ev = new CashDeposited(cmd)
            {
                AccountId = _accountId,
                DepositAmount = depositeAmount                
            };

            await _runner.Run(
                def => def.Given(accountCreated).When(cmd).Then(ev)
            );
        }

        [Fact]
        public async Task CashDepositAmountCannotBeNegative()
        {
            decimal depositeAmount = -5000;
            DateTime depositeDate = System.DateTime.Now;

            var accountCreated = new AccountCreated(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                AccountHolderName = "Tushar"
            };

            var cmd = new DepositCash
            {
                AccountId = _accountId,
                DepositAmount = depositeAmount                
            };

            await _runner.Run(
                def => def.Given(accountCreated).When(cmd).Throws(new ValidationException("Cash deposit amount cannot be negative"))
            );
        }

        [Fact]
        public async Task CashDepositShouldThrowExceptionWhenAccountIsNotPresent()
        {
            decimal depositeAmount = 5000;
            DateTime depositeDate = System.DateTime.Now;

            var cmd = new DepositCash
            {
                AccountId = _accountId,
                DepositAmount = depositeAmount                
            };

            await _runner.Run(
                def => def.Given().When(cmd).Throws(new ValidationException("No account with this ID exists"))
            );
        }

    }
}
