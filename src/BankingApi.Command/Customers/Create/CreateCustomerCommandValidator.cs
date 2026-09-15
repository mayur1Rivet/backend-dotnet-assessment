using FluentValidation;

namespace BankingApi.Command.Customers;

public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(x => x.Customer.FirstName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Customer.LastName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Customer.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(200);

        RuleFor(x => x.Customer.PhoneNumber)
            .MaximumLength(50)
            .When(x => !string.IsNullOrWhiteSpace(x.Customer.PhoneNumber));
    }
}
