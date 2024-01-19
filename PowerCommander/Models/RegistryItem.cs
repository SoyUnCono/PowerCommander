using Windows.Devices.Bluetooth.Advertisement;

namespace PowerCommander.Models;

public class RegistryItem
{
    /// <summary>
    /// Gets or sets the registry path associated with the item.
    /// </summary>
    public string? Path { get; set; }

    /// <summary>
    /// Gets or sets the name or key associated with the registry item.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the value associated with the registry item, including enable and revert values.
    /// </summary>
    public RegistryItemValue? Keyvalue { get; set; }

    /// <summary>
    /// Gets or sets a Type identifier for the registry item.
    /// </summary>
    public string? Type { get; set; }

}
