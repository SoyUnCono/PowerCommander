using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using PowerCommander.Contracts.Services;
using PowerCommander.Helpers;
using PowerCommander.Models;
namespace PowerCommander.ViewModels;

public partial class MainViewModel : ObservableRecipient
{
    #region Private Const properties

    /// <summary>
    /// Path to the JSON data file.
    /// </summary>
    private readonly string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "JSON", "SettingsElements.json");

    /// <summary>
    /// Service used for navigation purposes.
    /// </summary>
    private readonly INavigationService _navigationService;

    #endregion

    #region Public properties

    /// <summary>
    /// Displayed SecurityAndPrivacy tweaks on the view.
    /// </summary>
    [ObservableProperty] private List<SettingsItem>? _securityAndPrivacy;

    /// <summary>
    /// Displayed power tweaks on the view.
    /// </summary>
    [ObservableProperty] private List<SettingsItem>? _power;

    /// <summary>
    /// Displayed Gamemode tweaks on the view.
    /// </summary>
    [ObservableProperty] private List<SettingsItem>? _gamemodeTweaks;

    /// <summary>
    /// Displayed other tweaks on the view.
    /// </summary>
    [ObservableProperty] private List<SettingsItem>? _network;

    /// <summary>
    /// Displayed other tweaks on the view.
    /// </summary>
    [ObservableProperty] private List<SettingsItem>? _otherTweak;

    #endregion

    /// <summary>
    /// Main constructor for the MainViewModel class.
    /// </summary>
    public MainViewModel(INavigationService navigationService) =>
        // Initialize navigationService
        _navigationService = navigationService;

    #region private members

    /// <summary>
    /// Fetches JSON data from the specified file path, deserializes it into a list of SettingsGroups,
    /// and populates the provided 'targetSettingsList' with the 'Items' from a specific SettingsGroup identified by 'GroupName'.
    /// </summary>
    /// <param name="targetGroupName">The 'GroupName' to identify the specific SettingsGroup to iterate.</param>
    /// <param name="targetSettingsList">The list to populate with 'Items' from the target SettingsGroup.</param>
    private async Task FetchJSONData(string targetGroupName, List<SettingsItem> targetSettingsList)
    {
        try {

            // Read the content from the JSON file
            var jsonContent = File.ReadAllText(path);

            // Deserialize the JSON into a list of SettingsGroups
            var settingsGroups = JsonConvert.DeserializeObject<List<SettingsGroups>>(value: jsonContent);

            // Find the SettingsGroup with the specified 'GroupName'
            var targetGroup = settingsGroups?.FirstOrDefault(group => group?.GroupName == targetGroupName);

            // Check if the target group is found and has 'Items'
            if (targetGroup != null && targetGroup.Items != null) {
                // Add the 'Items' from the target group to the provided list
                targetSettingsList.AddRange(targetGroup.Items);
            }
            else {
                // Handle the case where the target group or its 'Items' is null
                await ContentDialogExtension.ShowDialogAsync(mTitle: "Oh no", mDescription: $"SettingsGroup with 'GroupName' '{targetGroupName}' not found or does not have 'Items'.", mCloseButtonText: "Ok", mPrimaryButtonText: "Retry");
            }
        }
        catch (Exception ex) {
            // Handle any exceptions that might occur during the process
            await ContentDialogExtension.ShowDialogAsync(mTitle: "Oh no", mDescription: $"A problem has ocurred while trying to load JSON file {ex.Message}", mCloseButtonText: "Ok", mPrimaryButtonText: "");
        }
    }

    /// <summary>
    /// Navigate to SettingsPage
    /// </summary>
    [RelayCommand]
    private void GoToSettingsViewModel() =>
        _navigationService.NavigateTo(typeof(SettingsViewModel).FullName!);

    /// <summary>
    /// Initializes the ViewModel asynchronously, sets various properties, and updates UI elements.
    /// </summary>
    [RelayCommand]
    private async void InitializeViewModelAsync()
    {
        // Initialize the SettingsCard list
        SecurityAndPrivacy = new List<SettingsItem>();
        Power = new List<SettingsItem>();
        GamemodeTweaks = new List<SettingsItem>();
        OtherTweak = new List<SettingsItem>();
        Network = new List<SettingsItem>();

        // Call the method to fetch the data into the listview
        await FetchJSONData("SecurityAndPrivacy", SecurityAndPrivacy);
        await FetchJSONData("Power", Power);
        await FetchJSONData("Gamemode", GamemodeTweaks);
        await FetchJSONData("OtherTweaks", OtherTweak);
        await FetchJSONData("Network", Network);
    }


    #endregion
}
