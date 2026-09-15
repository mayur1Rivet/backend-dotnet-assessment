using FluentValidation;

namespace BankingApi.Command.Accounts;

public class UpdateAccountCommandValidator : AbstractValidator<UpdateAccountCommand>
{
    public UpdateAccountCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0);

        RuleFor(x => x.Account)
            .NotNull();

        this.AddAccountFieldRules(
            x => x.Account.AccountType,
            x => x.Account.Balance);
    }
}
