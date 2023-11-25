using System;
using System.Collections.Generic;
using System.CommandLine.Parsing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ILogger = Microsoft.Extensions.Logging.ILogger;

public class ConsoleHostedService : IHostedService
{
    private readonly ILogger logger;
    private readonly IHostApplicationLifetime appLifetime;
    private readonly AppSettingsConfiguration config;
    private DatabaseService databaseService;
    private Parser parser;

    private int exitCode = -1;


    public ConsoleHostedService(
        ILogger<ConsoleHostedService> logger,
        IHostApplicationLifetime appLifetime,
        IOptions<AppSettingsConfiguration> config,
        DatabaseService databaseService,
        Parser parser
        )
    {
        this.logger = logger;
        this.appLifetime = appLifetime;
        this.config = config.Value;
        this.databaseService = databaseService;
        this.parser = parser;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        this.logger.LogDebug($"Starting with arguments: {string.Join(" ", Environment.GetCommandLineArgs())}");

        this.appLifetime.ApplicationStarted.Register(() =>
        {
            Task.Run(async () =>
            {
                try
                {
                    var args = Environment.GetCommandLineArgs();
                    this.logger.LogDebug($"Starting with arguments: {string.Join(" ", args)}");

                    await parser.InvokeAsync(args);

                    this.exitCode = 0;
                }
                catch (Exception ex)
                {
                    this.logger.LogError(ex, "Internal Program Error. Execution is canceled. Log Output in 'output.log'.");

                    this.exitCode = 1;

                }
                finally
                {
                    // Stop the application once the work is done
                    this.appLifetime.StopApplication();
                }
            });
        });

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {

        // Exit code may be null if the user cancelled via Ctrl+C/SIGTERM
        Environment.ExitCode = this.exitCode;
        return Task.CompletedTask;
    }
}