using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orders.Models;

namespace Orders.Services
{
    public class CustomerService : ICustomerService
    {
        private IList<Customer> _customer;

        public CustomerService() => _customer = new List<Customer> {
                new Customer(1, "Kewhi Leonard"),
                new Customer(2, "Serge Ibaka"),
                new Customer(3, "Pascal Siakam"),
                new Customer(4, "Danny Green"),
                new Customer(5, "Kyle Lowry")
            };

        public Customer GetCustomerById(int id)
        {
            return GetCustomerByIdAsync(id).Result;
        }

        public Task<Customer> GetCustomerByIdAsync(int id)
        {
            return Task.FromResult(_customer.Single(c => Equals(c.Id, id)));
        }

        public Task<IEnumerable<Customer>> GetCustomers()
        {
            return Task.FromResult(_customer.AsEnumerable());
        }
    }

    public interface ICustomerService
    {
        Customer GetCustomerById(int id);
        Task<Customer> GetCustomerByIdAsync(int id);
        Task<IEnumerable<Customer>> GetCustomers();
    }
}
