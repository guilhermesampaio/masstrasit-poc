using System;
using AspNetCore.MassTransit.Controllers;
using GreenPipes;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AspNetCore.MassTransit
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddMassTransit(x =>
            {
                //x.AddConsumer<RegionCreatedConsumer>();
                x.AddConsumer<FixedIncomeChangedConsumer>();

                x.AddBus(context => Bus.Factory.CreateUsingRabbitMq(config =>
                {
                    config.UseHealthCheck(context);
                    config.Host("rabbitmq://rabbitmq.hom.easynvest.io");
                   
                    //config.ReceiveEndpoint(nameof(RegionCreated), ep => 
                    //{
                    //    ep.PrefetchCount = 1;
                    //    ep.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(2)));
                    //    ep.ConfigureConsumer<RegionCreatedConsumer>(context);
                    //});

                    config.ReceiveEndpoint("Intranet.FixedIncomeChanged.AspNetCore.MassTransit", ep =>
                    {
                        //ep.ConfigureConsumeTopology = false;                        
                        ep.UseRawJsonSerializer();
                        
                        ep.Bind("Intranet.FixedIncomeChangedEvent.SymbolBaseFeeder.V1", e => 
                        {
                            e.AutoDelete = false;
                            e.Durable = true;
                            e.ExchangeType = "direct";
                            e.RoutingKey = "FixedIncomeChangedEvent";
                        });

                        ep.PrefetchCount = 1;                        
                        
                        ep.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(2)));                        
                        
                        ep.ConfigureConsumer<FixedIncomeChangedConsumer>(context);
                    });

                }));

                services.AddMassTransitHostedService();
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

   
}
