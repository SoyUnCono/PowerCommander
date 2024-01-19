namespace PowerCommander.Models;

public class RegistrySettings
{
    /// <summary>
    /// Gets or sets the name of the registry group.
    /// </summary>
    public string? RegistryGroupName { get; set; }

    /// <summary>
    /// Gets or sets the list of registry items associated with the registry group.
    /// </summary>
    public List<RegistryItem>? Items { get; set; }

}
