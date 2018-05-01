namespace CqrsAccount.Domain.Tests
{
    using Xunit;
    using System;
    using System.Threading.Tasks;
    using CqrsAccount.Domain.Tests.Common;
    using ReactiveDomain.Messaging;
    using Xunit.ScenarioReporting;

    [Collection("AggregateTest")]
    public class UC1AccountCreationTests : IDisposable
    {
        readonly Guid _accountId;
        readonly EventStoreScenarioRunner<Account> _runner;

        public UC1AccountCreationTests(EventStoreFixture fixture)
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
        public async Task CanCreateAccount()
        {
            var cmd = new CreateAccount
            {
                AccountId = _accountId,
                AccountHolderName = "Tushar"
            };

            var ev = new AccountCreated(cmd)
            {
                AccountId = cmd.AccountId,
                AccountHolderName = cmd.AccountHolderName
            };

            await _runner.Run(
                def => def.Given().When(cmd).Then(ev)
            );
        }

        [Fact]
        public async Task CannotCreateAccountWithNullName()
        {
            var cmd = new CreateAccount
            {
                AccountId = _accountId,
                AccountHolderName = null
            };

            await _runner.Run(
                def => def.Given().When(cmd).Throws(new ValidationException("A valid account owner name must be provided"))
            );
        }

        [Fact]
        public async Task CannotCreateAccountWithEmptyName()
        {
            var cmd = new CreateAccount
            {
                AccountId = _accountId,
                AccountHolderName = string.Empty
            };

            await _runner.Run(
                def => def.Given().When(cmd).Throws(new ValidationException("A valid account owner name must be provided"))
            );
        }

        [Fact]
        public async Task CannotCreateAccountWithWhiteSpaceName()
        {
            var cmd = new CreateAccount
            {
                AccountId = _accountId,
                AccountHolderName = " \t "
            };

            await _runner.Run(
                def => def.Given().When(cmd).Throws(new ValidationException("A valid account owner name must be provided"))
            );
        }

        [Fact]
        public async Task CannotCreateAccountWithSameId()
        {
            var created = new AccountCreated(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                AccountHolderName = "Tushar"
            };

            var cmd = new CreateAccount
            {
                AccountId = _accountId,
                AccountHolderName = "Tushar"
            };

            await _runner.Run(
                def => def.Given(created).When(cmd).Throws(new ValidationException("An account with this ID already exists"))
            );
        }
    }
}
