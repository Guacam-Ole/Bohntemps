// See https://aka.ms/new-console-template for more information
using Bohntemps.Models;

using Bohntemps;

using Microsoft.Extensions.DependencyInjection;
using BohnTemps.BeansApi;
using BohnTemps.Mastodon;
using Mastodon;
using Microsoft.Extensions.Logging;

var now=DateTime.Now;
Console.WriteLine("Bohntemps starting");


var services = new ServiceCollection();
services.AddScoped<Schedule>();
services.AddScoped<Communications>();
services.AddScoped<BeansConverter>();
services.AddScoped<Toot>(); 
services.AddScoped<Secrets>();

services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Debug);
    var logFile = "bohntemps.log";
    logging.AddFile(logFile, append: true);
});


var serviceProvider = services.BuildServiceProvider();
var converter = serviceProvider.GetRequiredService<BeansConverter>();

await converter.RetrieveAndSend();
Console.WriteLine($"Bohntemps finished. Tooks {(DateTime.Now-now).TotalSeconds} seconds");
