using GraphQL.Types;
using Orders.Models;
using Orders.Services;
using System;

namespace Orders.Schema
{
    public class OrdersMutation : ObjectGraphType<object>
    {
        public OrdersMutation(IOrderService orderService)
        {
            Name = "Mutation";
            Field<OrderType>("createOrder",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<OrderCreateInputType>> { Name = "order" }),
                resolve: context => {
                        var orderInput = context.GetArgument<OrderCreateInput>("order");
                        var id = Guid.NewGuid().ToString();
                        var order = new Order(id, orderInput.Name, orderInput.Description, orderInput.CreatedDate, orderInput.CustomerId);
                        return orderService.CreateAsync(order);
                });

            FieldAsync<OrderType>("startOrder",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "orderId" }),
                resolve: async context => {
                    var orderId = context.GetArgument<string>("orderId");
                    return await context.TryAsyncResolve(async c => await orderService.StartAsync(orderId));
                    });
        }
    }
}
