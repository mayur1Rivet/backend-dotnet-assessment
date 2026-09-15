using FluentValidation;

namespace BankingApi.Command.Customers;

public class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
{
    public UpdateCustomerCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0);

        this.AddCustomerFieldRules(
            x => x.Customer.FirstName,
            x => x.Customer.LastName,
            x => x.Customer.Email,
            x => x.Customer.PhoneNumber);
    }
}
