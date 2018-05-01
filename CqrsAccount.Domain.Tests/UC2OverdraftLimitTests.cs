namespace CqrsAccount.Domain.Tests
{
    using Xunit;
    using System;
    using System.Threading.Tasks;
    using CqrsAccount.Domain.Tests.Common;
    using ReactiveDomain.Messaging;
    using Xunit.ScenarioReporting;

    [Collection("AggregateTest")]
    public class UC2OverdraftLimitTests : IDisposable
    {
        readonly Guid _accountId;
        readonly EventStoreScenarioRunner<Account> _runner;

        public UC2OverdraftLimitTests(EventStoreFixture fixture)
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
        public async Task CanConfigureOverdraftLimitOnAccount()
        {
            decimal overdraftLimit = 500;
            var accountCreated = new AccountCreated(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                AccountHolderName = "Tushar"
            };
            var cmd = new ConfigureOverdraftLimit
            {
                AccountId = _accountId,
                OverdraftLimit = overdraftLimit
            };

            var ev = new OverdraftLimitConfigured(cmd)
            {
                AccountId = cmd.AccountId,
                OverdraftLimit = cmd.OverdraftLimit
            };

            await _runner.Run(
                def => def.Given(accountCreated).When(cmd).Then(ev)
            );
        }

        [Fact]
        public async Task CannotConfigureNegativeOverdraftLimitOnAccount()
        {
            decimal overdraftLimit = -5000;
            var accountCreated = new AccountCreated(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                AccountHolderName = "Tushar"
            };

            var cmd = new ConfigureOverdraftLimit
            {
                AccountId = _accountId,
                OverdraftLimit = overdraftLimit
            };

            await _runner.Run(
                def => def.Given(accountCreated).When(cmd).Throws(new ValidationException("Overdraft limit must be positive"))
            );
        }

        [Fact]
        public async Task ConfigureOverdraftLimitShouldThrowExceptionWhenAccountIsNotPresent()
        {
            decimal overdraftLimit = 500;
            var cmd = new ConfigureOverdraftLimit
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
