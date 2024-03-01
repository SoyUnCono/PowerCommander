using Microsoft.UI.Dispatching;
using Newtonsoft.Json;
using PowerCommander.Models;

namespace PowerCommander.Helpers;

public static class UpdateModelData 
{
    /// <summary>
    /// Updates the model data based on the state of the ToggleSwitch.
    /// </summary>
    /// <param name="uniqueID">The UniqueID to identify the SettingsItem.</param>
    /// <param name="toggleSwitchState">The state of the ToggleSwitch.</param>
    public static async void UpdateData(string uniqueID, bool toggleSwitchState)
    {
        using (HttpClient client = new()) {

            // Read the content from the JSON file containing settings items
            var settingsJsonContent = await client.GetStringAsync(Constants.UriConstants.SettingsElementsURL);

            // Deserialize the JSON into a list of SettingsGroups
            var settingsGroups = JsonConvert.DeserializeObject<List<SettingsGroups>>(settingsJsonContent);

            // Find the SettingsItem object corresponding to the UniqueID
            var settingsItemToUpdate = settingsGroups!
                .SelectMany(group => group.Items!)
                .FirstOrDefault(item => item.UniqueID == uniqueID);

            // Check if a valid SettingsItem object was found
            if (settingsItemToUpdate != null) {
                // Update the current value on the main Thread
                DispatcherQueue.GetForCurrentThread().TryEnqueue(() => {
                    // Update the ToggleSwitchState property of the SettingsItem object
                    settingsItemToUpdate.ToggleSwitchState=toggleSwitchState;
                });
            }
        }
    }


}
