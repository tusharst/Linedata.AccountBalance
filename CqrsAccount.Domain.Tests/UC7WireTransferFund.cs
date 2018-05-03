namespace CqrsAccount.Domain.Tests
{
    using Xunit;
    using System;
    using System.Threading.Tasks;
    using CqrsAccount.Domain.Tests.Common;
    using ReactiveDomain.Messaging;
    using Xunit.ScenarioReporting;

    [Collection("AggregateTest")]
    public class UC7WireTransferFund : IDisposable
    {
        readonly Guid _accountId;
        readonly EventStoreScenarioRunner<Account> _runner;

        public UC7WireTransferFund(EventStoreFixture fixture)
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
        public async Task CanTransferFundFromValidAccount()
        {
            decimal WireTransferFund = 5000;
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
            var cmd = new TransferWireFund()
            {
                AccountId = _accountId,
                WireFund = WireTransferFund
            };

            var ev = new WireFundTransferred(cmd)
            {
                AccountId = _accountId,
                WireFund = WireTransferFund
            };

            await _runner.Run(
                def => def.Given(accountCreated, evtCashDeposited).When(cmd).Then(ev)
            );
        }
    }
}
