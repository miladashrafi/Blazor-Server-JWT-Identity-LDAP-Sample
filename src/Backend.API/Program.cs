using Backend.API.Settings;
using Serilog;

namespace Backend.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        await CreateHostBuilder(args).Build().RunAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            })
            .ConfigureLogging(loggingBuilder => loggingBuilder.ClearProviders())
            .UseSerilog(SerilogOptions.ConfigureLogger);
}