// See https://aka.ms/new-console-template for more information
using Bohntemps.Models;

using Bohntemps;

using Microsoft.Extensions.DependencyInjection;
using BohnTemps.BeansApi;
using BohnTemps.Mastodon;
using Mastodon;

Console.WriteLine("Hello, World!");


var services = new ServiceCollection();
services.AddScoped<Schedule>();
services.AddScoped<Communications>();
services.AddScoped<BeansConverter>();
services.AddScoped<Toot>(); 
services.AddScoped<Secrets>();


var serviceProvider = services.BuildServiceProvider();
var converter = serviceProvider.GetRequiredService<BeansConverter>();

await converter.RetrieveAndSend();

