using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MachineStreamApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var webHost = BuildWebHost(args);
            var logger = webHost.Services.GetService<ILogger<Program>>();
            logger.LogInformation("Starting WebSocket listener...");

            var machineStreamListener = webHost.Services.GetService<MachineStreamListener>();
            machineStreamListener.Start();
            logger.LogInformation("WebSocket listener started.");

            logger.LogInformation("Starting the API...");
            webHost.Run();

            logger.LogInformation("Shutting down WebSocket listener...");
            machineStreamListener.Dispose();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                    logging.AddDebug();
                })
                .UseStartup<Startup>()
                .Build();
    }
}
