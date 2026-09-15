using BankingApi.Command.Customers;
using FluentValidation;

namespace BankingApi.Command.Auth;

public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        this.AddPasswordRules(command => command.User.Password);

        this.AddCustomerFieldRules(
            command => command.User.FirstName,
            command => command.User.LastName,
            command => command.User.Email,
            command => command.User.PhoneNumber);
    }
}