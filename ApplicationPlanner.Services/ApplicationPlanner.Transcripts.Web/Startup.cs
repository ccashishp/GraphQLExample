using GraphQL;
using ApplicationPlanner.Transcripts.Web.Configuration;
using Elmah.Io.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Orders.Schema;
using System;
using GraphQL.Server;
using Orders.Services;
using ApplicationPlanner.Transcripts.Web.Schema;

namespace ApplicationPlanner.Transcripts.Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration) => Configuration = configuration;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddJsonOptions(options =>
            {
                /*
                 * camelCase is now the default. No need for CamelCasePropertyNamesContractResolver
                 */
                options.SerializerSettings.Converters = new JsonConverter[]
                {
                    new Newtonsoft.Json.Converters.StringEnumConverter()
                };
                options.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                options.SerializerSettings.NullValueHandling = NullValueHandling.Include;
            });
            services.AddElmahIo(o =>
            {
                o.ApiKey = Configuration["ElmahIo:ApiKey"];
                o.LogId = new Guid(Configuration["ElmahIo:LogId"]);
            });
            services.AddCors();

            services.AddCoreDependencies(Configuration);
            services.AddTranscriptsDependencies(Configuration);
            this.ConfigureServicesGraphQl(services);
        }


        public void ConfigureServicesGraphQl(IServiceCollection services)
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

            services.AddSingleton<TranscriptRequestTypeEnum>();
            services.AddSingleton<TranscriptRequestStatusEnum>();
            services.AddSingleton<TranscriptRequestHistoryEventType>();
            services.AddSingleton<TranscriptResponseModelType>();
            services.AddSingleton<StudentType>();
            services.AddSingleton<TranscriptRequestMutation>();
            services.AddSingleton<TranscriptRequestQuery>();
            services.AddSingleton<TranscriptRequestsSchema>();

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
            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            app.UseElmahIo();
            app.UseMvc();

            
            app.UseGraphQL<OrdersSchema>("/graphql");
            app.UseGraphQL<TranscriptRequestsSchema>("/transcriptgraphql");
        }
    }
}
