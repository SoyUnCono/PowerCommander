using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PowerCommander.Models;
using static PowerCommander.Models.SettingsItem;

namespace PowerCommander.Helpers
{
    public class SettingsTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? SettingsCardTemplate { get; set; }
        public DataTemplate? SettingsExpanderTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (item is SettingsItem settingsItem) {
                switch (settingsItem.Type) {
                    case SettingsItemType.SettingsCard:
                        return SettingsCardTemplate!;
                    case SettingsItemType.SettingsExpander:
                        return SettingsExpanderTemplate!;
                }
            }

            return base.SelectTemplateCore(item);
        }
    }
}
