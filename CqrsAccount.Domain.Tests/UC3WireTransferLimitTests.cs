namespace CqrsAccount.Domain.Tests
{
    using Xunit;
    using System;
    using System.Threading.Tasks;
    using CqrsAccount.Domain.Tests.Common;
    using ReactiveDomain.Messaging;
    using Xunit.ScenarioReporting;

    [Collection("AggregateTest")]
    public class UC3WireTransferLimitTests : IDisposable
    {
        readonly Guid _accountId;
        readonly EventStoreScenarioRunner<Account> _runner;

        public UC3WireTransferLimitTests(EventStoreFixture fixture)
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
        public async Task CanConfigureDailyWireTransferLimitOnAccount()
        {
            decimal dailyWireTransferLimit = 5000;
            var accountCreated = new AccountCreated(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                AccountHolderName = "Tushar"
            };
            var cmd = new ConfigureDailyWireTransferLimit
            {
                AccountId = _accountId,
                DailyWireTransferLimit = dailyWireTransferLimit
            };

            var ev = new DailyWireTransferLimitConfigured(cmd)
            {
                AccountId = cmd.AccountId,
                DailyWireTransferLimit = cmd.DailyWireTransferLimit
            };

            await _runner.Run(
                def => def.Given(accountCreated).When(cmd).Then(ev)
            );
        }

        [Fact]
        public async Task CannotConfigureNegativeWireTransferLimitOnAccount()
        {
            decimal dailyWireTransferLimit = -5000;

            var accountCreated = new AccountCreated(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                AccountHolderName = "Tushar"
            };

            var cmd = new ConfigureDailyWireTransferLimit
            {
                AccountId = _accountId,
                DailyWireTransferLimit = dailyWireTransferLimit
            };

            await _runner.Run(
                def => def.Given(accountCreated).When(cmd).Throws(new ValidationException("Daily wire transfer limit cannot be negative"))
            );
        }

        [Fact]
        public async Task DailyWireTransferLimitShouldThrowExceptionWhenAccountIsNotPresent()
        {
            decimal dailyWireTransferLimit = 5000;

            var cmd = new ConfigureDailyWireTransferLimit
            {
                AccountId = _accountId,
                DailyWireTransferLimit = dailyWireTransferLimit
            };

            await _runner.Run(
                def => def.Given().When(cmd).Throws(new ValidationException("No account with this ID exists"))
            );
        }

    }
}
