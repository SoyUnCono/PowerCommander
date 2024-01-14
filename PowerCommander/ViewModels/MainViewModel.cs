using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using PowerCommander.Models;
namespace PowerCommander.ViewModels;

public partial class MainViewModel : ObservableRecipient
{
    #region Private Const properties

    /// <summary>
    /// Path to the JSON data file.
    /// </summary>
    private readonly string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "JSON", "SettingsElements.json");

    #endregion

    #region Public properties

    /// <summary>
    /// Displayed settings tweaks on the view.
    /// </summary>
    [ObservableProperty] private List<SettingsItem>? _aboutYourPC;

    /// <summary>
    /// Displayed settings tweaks on the view.
    /// </summary>
    [ObservableProperty] private List<SettingsItem>? _power;

    #endregion

    /// <summary>
    /// Main constructor for the MainViewModel class.
    /// </summary>
    public MainViewModel()
    {
        // Initialize the SettingsCard list
        AboutYourPC = new List<SettingsItem>();
        Power = new List<SettingsItem>();

        // Call the method to fetch the data into the listview
        FetchJSONData("SecurityAndPrivacy", AboutYourPC);

        // Call the method to fetch the data into the listview
        FetchJSONData("Power", Power);
    }

    #region private members

    /// <summary>
    /// Fetches JSON data from the specified file path, deserializes it into a list of SettingsGroups,
    /// and populates the provided 'targetSettingsList' with the 'Items' from a specific SettingsGroup identified by 'GroupName'.
    /// </summary>
    /// <param name="targetGroupName">The 'GroupName' to identify the specific SettingsGroup to iterate.</param>
    /// <param name="targetSettingsList">The list to populate with 'Items' from the target SettingsGroup.</param>
    private void FetchJSONData(string targetGroupName, List<SettingsItem> targetSettingsList)
    {
        try {
            // Read the content from the JSON file
            var jsonContent = File.ReadAllText(path);

            // Deserialize the JSON into a list of SettingsGroups
            var settingsGroups = JsonConvert.DeserializeObject<List<SettingsGroups>>(jsonContent);

            // Find the SettingsGroup with the specified 'GroupName'
            var targetGroup = settingsGroups?.FirstOrDefault(group => group?.GroupName == targetGroupName);

            // Check if the target group is found and has 'Items'
            if (targetGroup != null && targetGroup.Items != null) {
                // Add the 'Items' from the target group to the provided list
                targetSettingsList.AddRange(targetGroup.Items);
            }
            else {
                // Handle the case where the target group or its 'Items' is null
                Console.WriteLine($"SettingsGroup with 'GroupName' '{targetGroupName}' not found or does not have 'Items'.");
            }
        }
        catch (Exception ex) {
            // Handle any exceptions that might occur during the process
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }



    /// <summary>
    /// Initializes the ViewModel asynchronously, sets various properties, and updates UI elements.
    /// </summary>
    [RelayCommand]
    private void InitializeViewModelAsync()
    {

    }
    #endregion
}
