using GraphQL.Types;
using Orders.Services;

namespace Orders.Schema
{
    public class OrdersQuery : ObjectGraphType<object>
    {
        public OrdersQuery(IOrderService orderService)
        {
            Name = "Query";
            Field<ListGraphType<OrderType>>("orders", 
                resolve: context =>  orderService.GetOrdersAsync());
        }
    }
}
