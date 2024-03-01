using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PowerCommander.Helpers;

namespace PowerCommander.DataTemplates;

public sealed partial class SettingsCardTemplate : ResourceDictionary
{
    public SettingsCardTemplate()
    {
        this.InitializeComponent();
    }

    private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
    {
        var toggleSwitch = sender as ToggleSwitch;
        var uniqueID = toggleSwitch?.Tag?.ToString();

        if (!string.IsNullOrEmpty(uniqueID)) {
            // Set Toggle Switch State
            UpdateModelData.UpdateData(uniqueID, toggleSwitch!.IsOn);
        }
    }
}
