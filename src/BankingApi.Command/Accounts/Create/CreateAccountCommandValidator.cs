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

        this.AddAccountFieldRules(
            x => x.Account.AccountType,
            x => x.Account.Balance);
    }
}
