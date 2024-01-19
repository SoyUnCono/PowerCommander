namespace PowerCommander.Models;

public class RegistryItemValue
{
    /// <summary>
    /// Gets or sets the value to enable a specific feature or setting.
    /// </summary>
    public string? EnableValue { get; set; }

    /// <summary>
    /// Gets or sets the value to revert a specific feature or setting to its original state.
    /// </summary>
    public string? RevertValue { get; set; }
}
