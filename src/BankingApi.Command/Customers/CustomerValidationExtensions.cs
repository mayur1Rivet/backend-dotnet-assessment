using System.Linq.Expressions;
using FluentValidation;

namespace BankingApi.Command.Customers;

public static class CustomerValidationExtensions
{
    private const string NamePattern = @"^[A-Za-z]+(?:[ '-][A-Za-z]+)*$";
    private const string EmailPattern = @"^[A-Za-z0-9][A-Za-z0-9.!#$%&'*+/=?^_`{|}~-]*@[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?(?:\.[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?)+$";
    private const string IndianPhonePattern = @"^[6-9][0-9]{9}$";

    public static void AddCustomerFieldRules<T>(
        this AbstractValidator<T> validator,
        Expression<Func<T, string>> firstName,
        Expression<Func<T, string>> lastName,
        Expression<Func<T, string>> email,
        Expression<Func<T, string?>> phoneNumber)
    {
        var phoneNumberAccessor = phoneNumber.Compile();

        validator.RuleFor(firstName)
            .NotEmpty()
            .Matches(NamePattern)
            .WithMessage("First name can contain only letters, spaces, apostrophes, and hyphens.")
            .MaximumLength(100);

        validator.RuleFor(lastName)
            .NotEmpty()
            .Matches(NamePattern)
            .WithMessage("Last name can contain only letters, spaces, apostrophes, and hyphens.")
            .MaximumLength(100);

        validator.RuleFor(email)
            .NotEmpty()
            .Matches(EmailPattern)
            .WithMessage("Email must be a valid email address.")
            .MaximumLength(200);

        validator.RuleFor(phoneNumber)
            .Matches(IndianPhonePattern)
            .WithMessage("Phone number must be a valid Indian mobile number with exactly 10 digits.")
            .When(context => !string.IsNullOrWhiteSpace(phoneNumberAccessor(context)));
    }

    public static void AddPasswordRules<T>(
        this AbstractValidator<T> validator,
        Expression<Func<T, string>> password)
    {
        validator.RuleFor(password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches("[A-Z]")
            .WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]")
            .WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]")
            .WithMessage("Password must contain at least one digit.");
    }
}