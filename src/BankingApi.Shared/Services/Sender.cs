using BankingApi.Shared.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace BankingApi.Shared.Services;

public class Sender(IServiceProvider serviceProvider) : ISender
{
    public async Task<TResult> Send<TResult>(IRequest<TResult> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var requestType = request.GetType();
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(TResult));
        var handler = serviceProvider.GetService(handlerType);

        if (handler is null)
        {
            throw new InvalidOperationException($"No handler registered for request type '{requestType.Name}'.");
        }

        var method = handlerType.GetMethod(nameof(IRequestHandler<IRequest<TResult>, TResult>.Handle));
        if (method is null)
        {
            throw new InvalidOperationException($"Handler for {requestType.Name} does not expose Handle().");
        }

        var result = method.Invoke(handler, [request, cancellationToken]);
        if (result is Task<TResult> task)
        {
            return await task;
        }

        throw new InvalidOperationException($"Unexpected handler result type for {requestType.Name}.");
    }
}
