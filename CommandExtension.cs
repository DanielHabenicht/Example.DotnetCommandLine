using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RootCommand = System.CommandLine.RootCommand;

public static class CliCommandCollectionExtensions
{
    public static IServiceCollection AddCliCommands(this IServiceCollection services)
    {
        Type commandType = typeof(ICommandBase);

        var commands = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetExportedTypes())
            .Where(p => commandType.IsAssignableFrom(p) && !p.IsInterface);

        foreach (Type command in commands)
        {
            services.AddSingleton(commandType, command);
        }

        return services;
    }
}
