using System.Linq.Expressions;
using FluentValidation;

namespace BankingApi.Command.Accounts;

public static class AccountValidationExtensions
{
    public static void AddAccountFieldRules<T>(
        this AbstractValidator<T> validator,
        Expression<Func<T, string>> accountType,
        Expression<Func<T, decimal>> balance)
    {
        validator.RuleFor(accountType)
            .NotEmpty()
            .MaximumLength(50);

        validator.RuleFor(balance)
            .GreaterThanOrEqualTo(0m)
            .WithMessage("Balance cannot be negative.");
    }
}