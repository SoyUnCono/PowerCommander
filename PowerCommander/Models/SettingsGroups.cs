
namespace PowerCommander.Models;

public class SettingsGroups
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    public string? UniqueID { get; set; }

    /// <summary>
    /// Gets or sets the GroupName.
    /// </summary>
    public string? GroupName { get; set; }

    /// <summary>
    /// Gets or sets the list of configuration items within the group.
    /// </summary>
    public List<SettingsItem>? Items { get; set; }
}
