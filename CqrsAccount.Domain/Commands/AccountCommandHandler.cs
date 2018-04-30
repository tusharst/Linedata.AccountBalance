namespace CqrsAccount.Domain
{
    using System;
    using ReactiveDomain.Foundation;
    using ReactiveDomain.Messaging;
    using ReactiveDomain.Messaging.Bus;

    public sealed class AccountCommandHandler
        : IHandleCommand<CreateAccount>
        , IHandleCommand<SetOverdraftLimit>
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
                dispatcher.Subscribe<SetOverdraftLimit>(this)
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
        public CommandResponse Handle(SetOverdraftLimit command)
        {
            try
            {
                if (!_repository.TryGetById<Account>(command.AccountId, out var account))
                    throw new ValidationException("No account with this ID exists");

                 account.SetOverdraftLimit(command.OverdraftLimit, command);

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
