using BankingApi.Shared.Contracts;

namespace BankingApi.Command.Customers;

public record DeleteCustomerCommand(int Id) : IRequest<bool>;
