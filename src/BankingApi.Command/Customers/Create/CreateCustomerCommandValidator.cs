using FluentValidation;

namespace BankingApi.Command.Customers;

public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        this.AddCustomerFieldRules(
            x => x.Customer.FirstName,
            x => x.Customer.LastName,
            x => x.Customer.Email,
            x => x.Customer.PhoneNumber);
    }
}
