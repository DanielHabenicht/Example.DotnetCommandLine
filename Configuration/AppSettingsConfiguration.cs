using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


public class AppSettingsConfiguration
{
    public Dictionary<string, string>? ConnectionStrings { get; set; }

    public string? UseConnection { get; set; }

    [Range(0, int.MaxValue)] public int CommandTimeout { get; set; } = 30;

    public List<string>? DeleteContainers { get; set; }

    public List<string>? Task { get; set; }
}
