namespace CqrsAccount.Domain.Tests
{
    using Xunit;
    using System;
    using System.Threading.Tasks;
    using CqrsAccount.Domain.Tests.Common;
    using ReactiveDomain.Messaging;
    using Xunit.ScenarioReporting;

    [Collection("AggregateTest")]
    public class OverdraftLimitTests : IDisposable
    {
        readonly Guid _accountId;
        readonly EventStoreScenarioRunner<Account> _runner;

        public OverdraftLimitTests(EventStoreFixture fixture)
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

        [Theory]
        [InlineData(0)]
        [InlineData(10.60)]
        [InlineData(500)]
        public async Task CanSetOverdraftLimitOnAccount(decimal overdraftLimit)
        {
            var accountCreated = new AccountCreated(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                AccountHolderName = "Tushar"
            };
            var cmd = new SetOverdraftLimit
            {
                AccountId = _accountId,
                OverdraftLimit = overdraftLimit
            };

            var ev = new OverdraftLimitSet(cmd)
            {
                AccountId = cmd.AccountId,
                OverdraftLimit = cmd.OverdraftLimit
            };

            await _runner.Run(
                def => def.Given(accountCreated).When(cmd).Then(ev)
            );
        }

        [Theory]
        [InlineData(-10)]
        [InlineData(-410)]
        [InlineData(-9500)]
        public async Task CannotSetNegativeOverdraftLimitOnAccount(decimal overdraftLimit)
        {
            var accountCreated = new AccountCreated(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                AccountHolderName = "Tushar"
            };

            var cmd = new SetOverdraftLimit
            {
                AccountId = _accountId,
                OverdraftLimit = overdraftLimit
            };

            await _runner.Run(
                def => def.Given(accountCreated).When(cmd).Throws(new ValidationException("Overdraft limit must be positive"))
            );
        }

        [Theory]
        [InlineData(-10)]
        [InlineData(-410)]
        [InlineData(-9500)]
        public async Task CannotSetOverdraftLimitOnInvalidAccount(decimal overdraftLimit)
        {
            var cmd = new SetOverdraftLimit
            {
                AccountId = _accountId,
                OverdraftLimit = overdraftLimit
            };

            await _runner.Run(
                def => def.Given().When(cmd).Throws(new ValidationException("No account with this ID exists"))
            );
        }

    }
}
