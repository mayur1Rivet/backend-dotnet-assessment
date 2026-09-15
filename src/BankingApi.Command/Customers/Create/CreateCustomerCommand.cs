using BankingApi.DTO.Customers;
using BankingApi.Shared.Contracts;

namespace BankingApi.Command.Customers;

public record CreateCustomerCommand(CreateCustomerDto Customer) : IRequest<CustomerResponse>;
