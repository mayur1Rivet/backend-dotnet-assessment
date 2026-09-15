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

        RuleFor(x => x.Account.AccountType)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Account.Balance)
            .GreaterThanOrEqualTo(0m);
    }
}
