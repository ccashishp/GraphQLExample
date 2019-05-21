using GraphQL;
using GraphQL.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Orders.Schema;
using Orders.Services;

namespace Orders_GraphQL
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ICustomerService, CustomerService>();
            services.AddSingleton<IOrderService, OrderService>();
            services.AddSingleton<CustomerType>();
            services.AddSingleton<OrderType>();
            services.AddSingleton<OrderCreateInputType>();
            services.AddSingleton<OrderStatusesEnum>();
            services.AddSingleton<OrdersQuery>();
            services.AddSingleton<OrdersMutation>();
            services.AddSingleton<OrdersSchema>();

            // Allow dependencies to be injected for GraphQL.net
            services.AddSingleton<IDependencyResolver>(
                c => new FuncDependencyResolver(type => c.GetRequiredService(type)));

            services.AddGraphQL(options => {
                options.EnableMetrics = true;
                //options.ExposeExceptions = Environment.IsDevelopment();
            })
           .AddWebSockets()
           .AddDataLoader();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles(); // endpoint -> index.html
            app.UseStaticFiles(); // enable static file server
            app.UseWebSockets();
            app.UseGraphQLWebSockets<OrdersSchema>("/graphql");
            app.UseGraphQL<OrdersSchema>("/graphql");
        }
    }
}
