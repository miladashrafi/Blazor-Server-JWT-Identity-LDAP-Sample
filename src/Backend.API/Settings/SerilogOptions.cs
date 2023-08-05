using Serilog;
using Serilog.Debugging;
using Serilog.Enrichers.Span;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace Backend.API.Settings;

public static class SerilogOptions
{
    public static void ConfigureLogger(HostBuilderContext hostingContext, LoggerConfiguration loggerConfiguration)
    {
        SelfLog.Enable(Console.Error);
        loggerConfiguration.MinimumLevel.Verbose();
        loggerConfiguration.Enrich.FromLogContext();
        var applicationName = hostingContext.Configuration.GetValue<string>("Serilog:ApplicationName");
        loggerConfiguration.Enrich.WithProperty("ApplicationName", applicationName);
        loggerConfiguration.Enrich.WithSpan(new SpanOptions { IncludeTags = true, IncludeBaggage = true });


        loggerConfiguration.MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("System.Net.Http.HttpClient", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Model.Validation", LogEventLevel.Error);

        loggerConfiguration.WriteTo.Console(LogEventLevel.Error,
            theme: AnsiConsoleTheme.Code,
            outputTemplate:
            "[{Timestamp:HH:mm:ss} {Level:u3} <{SourceContext}>] {Message:lj} {Properties:j}{NewLine}{Exception}");
    }
}