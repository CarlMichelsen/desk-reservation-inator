using App;
using Microsoft.Extensions.Hosting;

Console.WriteLine("Starting Desk-Reservation-Inator");

var application = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingContext, config) =>
        config.RegisterApplicationConfiguration())
    .ConfigureServices((hostContext, services) =>
        services.RegisterApplicationServices(hostContext.Configuration));

await application.RunConsoleAsync();