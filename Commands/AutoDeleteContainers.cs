using System;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;

public class AutoDeleteContainers : Command
{

    public AutoDeleteContainers() : base("auto-delete-containers", "Automatically deletes old containers.")
    {
        var nameOption = new Option<string>("ageInDays")
        {
            Name = "--ageInDays",
            Description = "The age of the container in days.",
        };

        this.AddOption(nameOption);

        this.Handler = CommandHandler.Create((int days) => this.HandleCommand(days));
    }

    private int HandleCommand(int days)
    {
        try
        {
            Console.WriteLine($"Deleted Containers older than{days}!");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return 0;
        }

        return 1;
    }
}
