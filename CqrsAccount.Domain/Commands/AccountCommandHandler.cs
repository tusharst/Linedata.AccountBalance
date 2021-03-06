﻿namespace CqrsAccount.Domain
{
    using System;
    using ReactiveDomain.Foundation;
    using ReactiveDomain.Messaging;
    using ReactiveDomain.Messaging.Bus;

    public sealed class AccountCommandHandler
        : IHandleCommand<CreateAccount>
        , IHandleCommand<ConfigureOverdraftLimit>
        , IHandleCommand<ConfigureDailyWireTransferLimit>
        , IHandleCommand<DepositCheque>
        , IHandleCommand<DepositCash>
        , IHandleCommand<WithdrawCash>
        , IHandleCommand<TransferWireFund>
        , IDisposable
    {
        readonly IRepository _repository;
        readonly IDisposable _disposable;

        public AccountCommandHandler(IRepository repository, ICommandSubscriber dispatcher)
        {
            _repository = repository;

            _disposable = new CompositeDisposable
            {
                dispatcher.Subscribe<CreateAccount>(this),
                dispatcher.Subscribe<ConfigureOverdraftLimit>(this),
                dispatcher.Subscribe<ConfigureDailyWireTransferLimit>(this),
                dispatcher.Subscribe<DepositCheque>(this),
                dispatcher.Subscribe<DepositCash>(this),
                dispatcher.Subscribe<WithdrawCash>(this),
                dispatcher.Subscribe<TransferWireFund>(this)
            };
        }

        public void Dispose()
        {
            _disposable?.Dispose();
        }

        public CommandResponse Handle(CreateAccount command)
        {
            try
            {
                if (_repository.TryGetById<Account>(command.AccountId, out var _))
                    throw new ValidationException("An account with this ID already exists");

                var account = Account.Create(command.AccountId, command.AccountHolderName, command);

                _repository.Save(account);
                return command.Succeed();
            }
            catch (Exception e)
            {
                return command.Fail(e);
            }
        }
        public CommandResponse Handle(ConfigureOverdraftLimit command)
        {
            try
            {
                if (!_repository.TryGetById<Account>(command.AccountId, out var account))
                    throw new ValidationException("No account with this ID exists");

                 account.ConfigureOverdraftLimit(command.OverdraftLimit, command);

                _repository.Save(account);
                return command.Succeed();
            }
            catch (Exception e)
            {
                return command.Fail(e);
            }
        }
        public CommandResponse Handle(ConfigureDailyWireTransferLimit command)
        {
            try
            {
                if (!_repository.TryGetById<Account>(command.AccountId, out var account))
                    throw new ValidationException("No account with this ID exists");

                account.ConfigureDailyWireTransferLimit(command.DailyWireTransferLimit, command);

                _repository.Save(account);
                return command.Succeed();
            }
            catch (Exception e)
            {
                return command.Fail(e);
            }
        }

        public CommandResponse Handle(DepositCheque command)
        {
            try
            {
                if (!_repository.TryGetById<Account>(command.AccountId, out var account))
                    throw new ValidationException("No account with this ID exists");

                account.DepositChequeIntoAccount(command.DepositAmount,command.DepositDate, command);

                _repository.Save(account);
                return command.Succeed();
            }
            catch (Exception e)
            {
                return command.Fail(e);
            }
        }

        public CommandResponse Handle(DepositCash command)
        {
            try
            {
                if (!_repository.TryGetById<Account>(command.AccountId, out var account))
                    throw new ValidationException("No account with this ID exists");

                account.DepositCashIntoAccount(command.DepositAmount, command);

                _repository.Save(account);
                return command.Succeed();
            }
            catch (Exception e)
            {
                return command.Fail(e);
            }
        }
        public CommandResponse Handle(WithdrawCash command)
        {
            try
            {
                if (!_repository.TryGetById<Account>(command.AccountId, out var account))
                    throw new ValidationException("No account with this ID exists");

                account.WithdrawCashFromAccount(command.WithdrawAmount, command);

                _repository.Save(account);
                return command.Succeed();
            }
            catch (Exception e)
            {
                return command.Fail(e);
            }
        }
        public CommandResponse Handle(TransferWireFund command)
        {
            try
            {
                if (!_repository.TryGetById<Account>(command.AccountId, out var account))
                    throw new ValidationException("No account with this ID exists");

                account.TransferWireFund(command.WireFund, command);

                _repository.Save(account);
                return command.Succeed();
            }
            catch (Exception e)
            {
                return command.Fail(e);
            }
        }
    }
}
