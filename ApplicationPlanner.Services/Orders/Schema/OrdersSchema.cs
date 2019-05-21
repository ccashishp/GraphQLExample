using GraphQL;

namespace Orders.Schema
{
    public class OrdersSchema : GraphQL.Types.Schema
    {
        public OrdersSchema(OrdersQuery ordersQuery, OrdersMutation ordersMutation, IDependencyResolver dependencyResolver)
        {
            Query = ordersQuery;
            Mutation = ordersMutation;
            DependencyResolver = dependencyResolver;
        }
    }
}
