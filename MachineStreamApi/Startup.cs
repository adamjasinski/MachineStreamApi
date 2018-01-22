using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MachineStreamApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var mongoDbConnectionString = Configuration["MongoDBConnectionString"];
            var webSocketsUrl = Configuration["WebSocketsUrl"];
            services.AddSingleton<EventRepository>(new EventRepository(mongoDbConnectionString));
            services.AddTransient<MachineStreamListener>(
                x => new MachineStreamListener(webSocketsUrl,
                    x.GetService<EventRepository>(), x.GetService<ILogger<MachineStreamListener>>()));

            services.AddMvc();
            services.AddLogging();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
