using BankingApi.DTO.Customers;
using BankingApi.Shared.Contracts;

namespace BankingApi.Command.Customers;

public record UpdateCustomerCommand(int Id, UpdateCustomerDto Customer) : IRequest<CustomerResponse>;
