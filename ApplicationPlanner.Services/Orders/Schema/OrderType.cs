using GraphQL.Types;
using Orders.Models;
using Orders.Services;

namespace Orders.Schema
{
    public class OrderType : ObjectGraphType<Order>
    {
        public OrderType(ICustomerService customerService)
        {
            Field(o => o.Id);
            Field(o => o.Name);
            Field(o => o.Description);
            Field(o => o.CreatedDate);
            Field<CustomerType>("customer", resolve: context => customerService.GetCustomerByIdAsync(context.Source.CustomerId));
            Field<OrderStatusesEnum>("status", resolve: context => context.Source.Status);
        }
    }
}
