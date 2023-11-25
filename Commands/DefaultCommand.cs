using System;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using Microsoft.Extensions.Logging;

public class DefaultCommand : System.CommandLine.RootCommand
{
    private ILogger logger;

    public DefaultCommand(ILogger<DefaultCommand> logger)  : base( "Default execution in an interactive way.")
    {
        this.logger = logger;
        var nameOption = new Option<string>("ageInDays")
        {
            Name = "--ageInDays",
            ArgumentHelpName = "int",
            Description = "The age of the container in days.",
            IsRequired = false
        };

        this.AddOption(nameOption);

        this.Handler = CommandHandler.Create((int days) => this.HandleCommand(days));
    }

    public Option<string> Option1 { get; } = new("--option1", () => "abc", "first option");

    private int HandleCommand(int days)
    {
        try
        {
            this.logger.LogInformation("Hi");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return 0;
        }

        return 1;
    }
}

