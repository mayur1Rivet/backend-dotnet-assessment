using FluentValidation;

namespace BankingApi.Command.Auth;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(command => command.User.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(command => command.User.Password)
            .NotEmpty();
    }
}