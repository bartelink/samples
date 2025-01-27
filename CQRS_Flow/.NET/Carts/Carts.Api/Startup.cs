using Core;
using Core.EventStoreDB;
using Core.WebApi.Middlewares.ExceptionHandling;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;

namespace Carts.Api
{
    public class Startup
    {
        private readonly IConfiguration config;

        public Startup(IConfiguration config)
        {
            this.config = config;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .AddNewtonsoftJson(opt => opt.SerializerSettings.Converters.Add(new StringEnumConverter()));

            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "Carts", Version = "v1"});
            });

            services
                .AddEventStoreDBSubscriptionToAll("MainSubscription")
                .AddCoreServices()
                .AddCartsModule(config);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware(typeof(ExceptionHandlingMiddleware));

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Carts V1");
                c.RoutePrefix = string.Empty;
            });
        }
    }
}
