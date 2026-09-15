using BankingApi.Command.Accounts;
using BankingApi.Command.Auth;
using BankingApi.Command.Customers;
using BankingApi.AppService.Auth;
using BankingApi.DTO.Accounts;
using BankingApi.DTO.Auth;
using BankingApi.DTO.Customers;
using BankingApi.Infrastructure.Entity;
using BankingApi.Query.Accounts;
using BankingApi.Query.Customers;
using BankingApi.Shared.Contracts;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace BankingApi.AppService.Extensions;

public static class AppServiceExtensions
{
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddScoped<IRequestHandler<CreateCustomerCommand, CustomerResponse>, CreateCustomerCommandHandler>();
        services.AddScoped<IRequestHandler<RegisterCommand, AuthResponse>, RegisterCommandHandler>();
        services.AddScoped<IRequestHandler<LoginCommand, AuthResponse>, LoginCommandHandler>();
        services.AddScoped<IRequestHandler<UpdateCustomerCommand, CustomerResponse>, UpdateCustomerCommandHandler>();
        services.AddScoped<IRequestHandler<DeleteCustomerCommand, bool>, DeleteCustomerCommandHandler>();
        services.AddScoped<IRequestHandler<GetAllCustomersQuery, IEnumerable<CustomerResponse>>, GetAllCustomersQueryHandler>();
        services.AddScoped<IRequestHandler<GetCustomerByIdQuery, CustomerResponse?>, GetCustomerByIdQueryHandler>();

        services.AddScoped<IRequestHandler<CreateAccountCommand, AccountResponse>, CreateAccountCommandHandler>();
        services.AddScoped<IRequestHandler<UpdateAccountCommand, AccountResponse>, UpdateAccountCommandHandler>();
        services.AddScoped<IRequestHandler<DeleteAccountCommand, bool>, DeleteAccountCommandHandler>();
        services.AddScoped<IRequestHandler<GetAllAccountsQuery, IEnumerable<AccountResponse>>, GetAllAccountsQueryHandler>();
        services.AddScoped<IRequestHandler<GetAccountByIdQuery, AccountResponse?>, GetAccountByIdQueryHandler>();
        services.AddScoped<IRequestHandler<GetAccountsByCustomerIdQuery, IEnumerable<AccountResponse>>, GetAccountsByCustomerIdQueryHandler>();

        services.AddValidatorsFromAssemblyContaining<CreateCustomerCommandValidator>();
        services.AddValidatorsFromAssemblyContaining<CreateAccountCommandValidator>();
        services.AddSingleton<IAuthTokenService, JwtTokenService>();
        services.AddSingleton<Microsoft.AspNetCore.Identity.IPasswordHasher<User>, Microsoft.AspNetCore.Identity.PasswordHasher<User>>();

        return services;
    }
}
