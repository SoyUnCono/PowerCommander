using PowerCommander.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PowerCommander.Contracts.Services
{
    public interface IFetchJSONDataService
    {
        // Fetches JSON data from the specified file path, deserializes it into a list of SettingsGroups,
        // and populates the provided 'targetSettingsList' with the
        // 'Items' from a specific SettingsGroup identified by 'GroupName'.
        Task<List<SettingsItem>> InyectDataToList(string targetGroupName);

        // Searches for a specific UniqueID within the settings groups
        // and applies registry settings if a matching UniqueID is found
        // and the associated ToggleSwitch is enabled.
        Task SearchForUniqueID(string registryGroupName);

        // Applies registry settings based on the information provided in a
        // JSON file for a specific UniqueID.
        Task ApplyRegistrySettingsForUniqueID(string registryGroupName);

        // Refreshes the data.
        Task RefreshData();

    }
}
