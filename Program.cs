using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Extensions.Logging;
using Directory = System.IO.Directory;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using Log = Serilog.Log;

public class Program
{
    // private static ILoggerFactory loggerFactory;
    // private static ContextService contextFactory;
    // private static IConfigurationRoot config;

    private static async Task Main(string[] args)
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "";
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .AddCommandLine(args)
            .Build();

        //logging
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(config)
            .MinimumLevel.Debug()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.Debug()
            .WriteTo.File("output.log")
            .CreateLogger();

        // get config
        Log.Logger.Information("Environment: " + env);

        var builder = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                // Konfiguration der App-Einstellungen und Validierung.
                services.ConfigureAndValidate<AppSettingsConfiguration>(options =>
                {
                    context.Configuration.Bind(options);
                });


                // services.AddSingleton(
                //     serviceProvider =>
                //     {
                //         var configuration = serviceProvider.GetService<IOptions<AppSettingsConfiguration>>().Value;
                //         var logger = serviceProvider.GetService<ILogger<Program>>();

                //         var connection = configuration.UseConnection ?? "DefaultConnection";
                //         configuration.ConnectionStrings.TryGetValue(connection, out var connectionString);

                //         if (connectionString == null)
                //         {
                //             throw new ArgumentNullException(
                //                 $"No Connection String given for Connection '{connection}'");
                //         }

                //         var commandTimeout = TimeSpan.FromSeconds(configuration.CommandTimeout);

                //         var contextFactory = new ContextService(connectionString, commandTimeout, logger);
                //         var context = contextFactory.CreateContext();
                //         Log.Logger.Information($"Each sql command should be executed within {context.Database.GetCommandTimeout()} seconds.");

                //         return context;
                //     });
                // services.AddDbContext("connectionString");

                services.AddSingleton<DatabaseService>();
                services.AddSingleton<DefaultCommand>();
                services.AddSingleton<AutoDeleteContainers>();

                // services.AddCliCommands();

                services.AddSingleton(serviceProvider =>
                {
                    var rootCommand = serviceProvider.GetService<DefaultCommand>();
                    foreach (Command command in serviceProvider.GetServices<Command>())
                    {
                        rootCommand.AddCommand(command);
                    }
                    var commandLineBuilder = new CommandLineBuilder(rootCommand);


                    return commandLineBuilder.UseDefaults().Build();
                });

                services.AddHostedService<ConsoleHostedService>();

            })
            .UseSerilog();




        // Start Console
        await builder.RunConsoleAsync();

    }
}
