using Microsoft.UI.Xaml;

namespace PowerCommander.Models;

public class SettingsItem
{
    /// <summary>
    /// Gets or sets the type of the settings item .
    /// </summary>
    public enum SettingsItemType
    {
        SettingsCard,
        SettingsExpander
    }

    /// <summary>
    /// Gets or sets the type of the settings item (e.g., "SettingsCard" or "SettingsExpander").
    /// </summary>
    public SettingsItemType Type { get; set; }

    /// <summary>
    /// Gets or sets the title of the settings item.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the description of the settings item.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the icon associated with the settings item.
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the settings item has an action icon.
    /// </summary>
    public bool HasActionIcon { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether clicking is enabled for the settings item.
    /// </summary>
    public bool IsClickEnable { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the settings item is enabled.
    /// </summary>
    public bool IsEnable { get; set; }

    /// <summary>
    /// Gets or sets a value indicating the toggle switch state of the settings item.
    /// </summary>
    public bool ToggleSwitchState { get; set; }

    /// <summary>
    /// Gets or sets a unique identifier for the settings item.
    /// </summary>
    public string? UniqueID { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the settings expander is expanded.
    /// </summary>
    public bool IsExpanded { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the notification is currently open.
    /// </summary>
    public bool Notification_IsOpen { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the notification can be closed.
    /// </summary>
    public bool Notification_IsClosable { get; set; }

    /// <summary>
    /// Gets or sets the title of the notification.
    /// </summary>
    public string? Notification_Title { get; set; }

    /// <summary>
    /// Gets or sets the message content of the notification.
    /// </summary>
    public string? Notification_Message { get; set; }

    /// <summary>
    /// Gets or sets the visibility of the expander switch.
    /// </summary>
    public bool ExpanderSwitchVisibility { get; set; }

    /// <summary>
    /// Gets or sets the state of the expander switch.
    /// </summary>
    public bool ExpanderSwitchState { get; set; }

    /// <summary>
    /// Gets or sets a list of settings items within the expander (if applicable).
    /// </summary>
    public List<SettingsItem>? SettingsExpanderItem { get; set; }

    /// <summary>
    /// Main Constructor
    /// </summary>
    public SettingsItem()
    {
        // Set Notification Properties
        // Indicates whether the notification is open or closed
        Notification_IsOpen = false;

        // Indicates whether the notification can be closed
        Notification_IsClosable = false;

        // Enables the notification
        IsEnable = true;

        // Indicates whether the notification has an action icon
        HasActionIcon = false;

    }
}
