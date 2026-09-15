using BankingApi.Shared.Contracts;
using BankingApi.Shared.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BankingApi.Shared.Extensions;

public static class SharedExtensions
{
    public static IServiceCollection AddSharedServices(this IServiceCollection services)
    {
        services.AddScoped<ISender, Sender>();
        return services;
    }
}
