// See https://aka.ms/new-console-template for more information

using System.Reflection;
using Bohntemps.Models;
using Bohntemps;
using Microsoft.Extensions.DependencyInjection;
using BohnTemps.BeansApi;
using BohnTemps.Mastodon;
using Mastodon;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Sinks.Grafana.Loki;

var now = DateTime.Now;
Console.WriteLine("Bohntemps starting");
const int maxRetries = 5;

var services = new ServiceCollection();
services.AddScoped<Schedule>();
services.AddScoped<Communications>();
services.AddScoped<BeansConverter>();
services.AddScoped<Toot>();
services.AddScoped<Secrets>();

services.AddLogging(cfg => cfg.SetMinimumLevel(LogLevel.Debug));
services.AddSerilog(cfg =>
{
    cfg.MinimumLevel.Debug()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("job", Assembly.GetEntryAssembly()?.GetName().Name)
        .Enrich.WithProperty("service", Assembly.GetEntryAssembly()?.GetName().Name)
        .Enrich.WithProperty("desktop", Environment.GetEnvironmentVariable("DESKTOP_SESSION"))
        .Enrich.WithProperty("language", Environment.GetEnvironmentVariable("LANGUAGE"))
        .Enrich.WithProperty("lc", Environment.GetEnvironmentVariable("LC_NAME"))
        .Enrich.WithProperty("timezone", Environment.GetEnvironmentVariable("TZ"))
        .Enrich.WithProperty("dotnetVersion", Environment.GetEnvironmentVariable("DOTNET_VERSION"))
        .Enrich.WithProperty("inContainer",
            Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"))
        .WriteTo.GrafanaLoki(Environment.GetEnvironmentVariable("LOKIURL") ?? "http://thebeast:3100",
            propertiesAsLabels: ["job"]);
    if (Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyConfigurationAttribute>()?.Configuration ==
        "Debug")
    {
        cfg.WriteTo.Console(new RenderedCompactJsonFormatter());
    }
    else
    {
        cfg.WriteTo.Console();
    }
});

var serviceProvider = services.BuildServiceProvider();
var converter = serviceProvider.GetRequiredService<BeansConverter>();

var retries = maxRetries;
while (true)
{
    try
    {
        Thread.Sleep(1000 * 60 * 5);
        retries--;
        await converter.RetrieveAndSend();
        Console.WriteLine($"Bohntemps finished. Tooks {(DateTime.Now - now).TotalSeconds} seconds");
        retries = maxRetries;
    }
    catch (Exception e)
    {
        Console.WriteLine($"{retries}:{e.Message}");
        if (retries == 0) throw;
    }
}