using BankingApi.Infrastructure.IRepository;
using BankingApi.Shared.Contracts;

namespace BankingApi.Command.Accounts;

public class DeleteAccountCommandHandler(IAccountRepository accountRepository)
    : IRequestHandler<DeleteAccountCommand, bool>
{
    public async Task<bool> Handle(DeleteAccountCommand request, CancellationToken cancellationToken = default)
    {
        var account = await accountRepository.GetByIdAsync(request.Id, cancellationToken);
        if (account is null)
        {
            return false;
        }

        await accountRepository.DeleteAsync(account, cancellationToken);
        await accountRepository.SaveChangesAsync(cancellationToken);
        return true;
    }
}
