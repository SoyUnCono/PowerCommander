using Microsoft.UI.Dispatching;
using Microsoft.Win32;
using Newtonsoft.Json;
using PowerCommander.Contracts.Services;
using PowerCommander.Helpers;
using PowerCommander.Models;

namespace PowerCommander.Services;

public class FetchJSONDataService : IFetchJSONDataService
{
    /// <summary>
    /// DispatcherQueue instance for the current thread.
    /// </summary>
    private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

    /// <summary>
    /// Fetches JSON data from the specified file path, deserializes it into a list of SettingsGroups,
    /// and populates the provided 'targetSettingsList' with the 'Items' from a specific SettingsGroup identified by 'GroupName'.
    /// </summary>
    /// <param name="targetGroupName">The 'GroupName' to identify the specific SettingsGroup to iterate.</param>
    /// <param name="targetSettingsList">The list to populate with 'Items' from the target SettingsGroup.</param>
    public async Task<List<SettingsItem>> InyectDataToList(string targetGroupName)
    {
        List<SettingsItem> targetSettingsList = new();

        using (HttpClient client = new HttpClient()) {
            try {
                // Read the content from the JSON file
                var jsonContent = await client.GetStringAsync(Constants.UriConstants.SettingsElementsURL);

                // Deserialize the JSON into a list of SettingsGroups
                var settingsGroups = JsonConvert.DeserializeObject<List<SettingsGroups>>(jsonContent);

                // Find the SettingsGroup with the specified 'GroupName'
                var targetGroup = settingsGroups?.FirstOrDefault(group => group?.GroupName == targetGroupName);

                // Check if the target group and its items are not null
                if (targetGroup != null && targetGroup.Items != null) {
                    // Iterate through each item in the target group
                    foreach (var item in targetGroup.Items) {
                        // Set the ToggleSwitchState to the default value based on the UniqueID
                        item.ToggleSwitchState = DefaultToggleSwitchStates.GetDefaultState(item.UniqueID!);
                    }

                    // Add the 'Items' from the target group to the provided list
                    targetSettingsList.AddRange(targetGroup.Items);
                }
            }
            catch (Exception ex) {
                // Handle any exceptions that might occur during the process
                await ContentDialogExtension.ShowDialogAsync(mTitle: "PowerCommander", mDescription: $"A problem has occurred while trying to load JSON file {ex.Message}", mCloseButtonText: "Ok", mPrimaryButtonText: "");
            }

            return targetSettingsList;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task RefreshData() =>
        throw new NotImplementedException();


    /// <summary>
    /// Searches for a specific UniqueID within the settings groups and applies registry settings if a matching UniqueID is found and the associated ToggleSwitch is enabled.
    /// </summary>
    /// <param name="mRegistryGroupName">The UniqueID to search for within the settings groups.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task SearchForUniqueID(string registryGroupName)
    {
        using (HttpClient client = new HttpClient()) {
            try {
                // Read the content from the JSON file containing settings items
                var settingsJsonContent = await client.GetStringAsync(Constants.UriConstants.SettingsElementsURL);

                // Deserialize the JSON into a list of SettingsGroups
                var settingsGroups = JsonConvert.DeserializeObject<List<SettingsGroups>>(settingsJsonContent);

                if (settingsGroups != null) {
                    // Find the target UniqueID in the list of settings groups
                    var mTargetUniqueID = settingsGroups
                    .SelectMany(group => group.Items!)
                    .FirstOrDefault(item => item.UniqueID == registryGroupName);

                    // Check if there is at least one ToggleSwitch that is enabled
                    if (mTargetUniqueID != null && mTargetUniqueID.ToggleSwitchState == true) {
                        // After finding the UniqueID, apply the specific registry settings
                        await ApplyRegistrySettingsForUniqueID(registryGroupName);
                    }

                }
            }
            catch (Exception ex) {
                // Handle any exceptions that might occur during the process
                await ContentDialogExtension.ShowDialogAsync(mTitle: "PowerCommander", mDescription: $"A problem has occurred while trying to load JSON file {ex.Message}", mCloseButtonText: "Ok", mPrimaryButtonText: "");
            }
        }
    }

    /// <summary>
    /// Applies registry settings based on the information provided in a JSON file for a specific UniqueID.
    /// </summary>
    /// <param name="mRegistryGroupName">UniqueID used to filter registry settings.</param>
    public async Task ApplyRegistrySettingsForUniqueID(string registryGroupName)
    {
        using (HttpClient client = new HttpClient()) {
            try {
                // Read the content from the JSON file of the registry
                var registryJsonContent = await client.GetStringAsync(Constants.UriConstants.RegistryElementsURL);

                // Deserialize the JSON into a list of RegistrySettings
                var registrySettings = JsonConvert.DeserializeObject<List<RegistrySettings>>(registryJsonContent);

                // Filter the list of RegistrySettings to get only those with RegistryGroupName equal to the UniqueID
                var filteredRegistrySettings = registrySettings?
                    .Where(registrySetting => registrySetting.RegistryGroupName == registryGroupName)
                    .ToList();

                if (filteredRegistrySettings != null) {
                    // Iterate over the filtered RegistrySettings
                    foreach (var registrySetting in filteredRegistrySettings) {
                        // Iterate over the Items in each RegistrySetting
                        foreach (var item in registrySetting.Items!) {
                            try {
                                // Base registry key variable
                                RegistryKey? mBaseKey = Registry.LocalMachine;

                                // Determine the base of the registry based on the path specified in the JSON
                                switch (item.Path) {
                                    case string path when path.StartsWith("HKEY_LOCAL_MACHINE"):
                                        mBaseKey = Registry.LocalMachine;
                                        break;
                                    case string path when path.StartsWith("USER_CURRENT"):
                                        mBaseKey = Registry.CurrentUser;
                                        break;
                                    default:
                                        // Default case
                                        break;
                                }

                                // Get the rest of the path after the root
                                string subKeyPath = item.Path!.Substring(item.Path.IndexOf("\\") + 1);

                                // Open the registry key
                                var registryKey = mBaseKey!.OpenSubKey(subKeyPath, true);

                                if (registryKey != null) {

                                    // Check if the key already exists
                                    var existingValue = registryKey.GetValue(item.Name);

                                    if (existingValue != null) {
                                        // If the key exists, delete it before adding it again
                                        registryKey.DeleteValue(item.Name!);
                                    }

                                    // Determine the type of the registry based on the registry specified in the JSON
                                    switch (item.Type) {
                                        case "DWORD":
                                            registryKey.SetValue(item.Name, int.Parse(item?.Keyvalue?.EnableValue!), RegistryValueKind.DWord);
                                            break;
                                        default:
                                            await ContentDialogExtension.ShowDialogAsync(mTitle: "PowerCommander", mDescription: $"Registry-specific type doesn't exist or is not matched", mCloseButtonText: "Ok", mPrimaryButtonText: "");
                                            break;
                                    }

                                }
                            }
                            catch (Exception ex) {
                                // Handle any exceptions that might occur during the process
                                await ContentDialogExtension.ShowDialogAsync(mTitle: "PowerCommander", mDescription: $"A problem has occurred while trying to apply settings to the registry {ex.Message}", mCloseButtonText: "Ok", mPrimaryButtonText: "");
                            }
                        }
                    }
                }
            }
            catch (Exception ex) {
                // Handle any exceptions that might occur during the process
                await ContentDialogExtension.ShowDialogAsync(mTitle: "PowerCommander", mDescription: $"A problem has occurred while trying to load JSON file {ex.Message}", mCloseButtonText: "Ok", mPrimaryButtonText: "");
            }
        }
    }

    
}
