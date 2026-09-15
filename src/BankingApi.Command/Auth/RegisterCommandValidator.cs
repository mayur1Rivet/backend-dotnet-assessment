using BankingApi.Command.Customers;
using FluentValidation;

namespace BankingApi.Command.Auth;

public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(command => command.User.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches("[A-Z]")
            .WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]")
            .WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]")
            .WithMessage("Password must contain at least one digit.");

        this.AddCustomerFieldRules(
            command => command.User.FirstName,
            command => command.User.LastName,
            command => command.User.Email,
            command => command.User.PhoneNumber);
    }
}