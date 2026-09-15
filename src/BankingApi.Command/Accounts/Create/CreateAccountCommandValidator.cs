using BankingApi.DTO.Accounts;
using FluentValidation;

namespace BankingApi.Command.Accounts;

public class CreateAccountCommandValidator : AbstractValidator<CreateAccountCommand>
{
    public CreateAccountCommandValidator()
    {
        RuleFor(x => x.Account)
            .NotNull();

        RuleFor(x => x.Account.CustomerId)
            .GreaterThan(0);

        RuleFor(x => x.Account.AccountType)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Account.Balance)
            .GreaterThanOrEqualTo(0m);
    }
}
