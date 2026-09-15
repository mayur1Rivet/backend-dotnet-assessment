namespace BankingApi.Shared.Contracts;

public interface ISender
{
    Task<TResult> Send<TResult>(
        IRequest<TResult> request,
        CancellationToken cancellationToken = default);
}
