using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.Win32;
using Newtonsoft.Json;
using PowerCommander.Helpers;
using PowerCommander.Models;
using Windows.System;
using WinUICommunity;
namespace PowerCommander.ViewModels;

public partial class MainViewModel : ObservableRecipient
{
    #region Private Const properties

    /// <summary>
    /// Path to the JSON data file.
    /// </summary>
    private readonly string mSettingsElements = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "JSON", "SettingsElements.json");

    /// <summary>
    /// Path to the JSON Registry file.
    /// </summary>
    private readonly string mRegistry = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "JSON", "Registry.json");

    /// <summary>
    /// Service used for navigation purposes.
    /// </summary>
    private readonly Contracts.Services.INavigationService _navigationService;

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
    /// Displayed Network tweaks on the view.
    /// </summary>
    [ObservableProperty] private List<SettingsItem>? _network;

    /// <summary>
    /// Displayed other tweaks on the view.
    /// </summary>
    [ObservableProperty] private List<SettingsItem>? _otherTweak;

    /// <summary>
    /// Custom text displayed to the user along with the current enumerated tweaks.
    /// </summary>
    [ObservableProperty] private string? _placeHolderSuggestions;

    /// <summary>
    /// The current count of tweaks in the JSON file.
    /// </summary>
    [ObservableProperty] private int _totalTweaks;

    /// <summary>
    /// Represents the current profile picture associated with the local user account.
    /// </summary>
    [ObservableProperty] private ImageSource? _accountPicture;

    /// <summary>
    /// Gets or sets the email associated with the user's Microsoft account.
    /// </summary>
    [ObservableProperty] private string? _email;

    /// <summary>
    /// Gets or sets the username or display name associated with the user's Microsoft account.
    /// </summary>
    [ObservableProperty] private string? _username;

    #endregion

    /// <summary>
    /// Main constructor for the MainViewModel class.
    /// </summary>
    public MainViewModel(Contracts.Services.INavigationService navigationService) =>
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
            var jsonContent = File.ReadAllText(mSettingsElements);

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
                await ContentDialogExtension.ShowDialogAsync(mTitle: "PowerCommander", mDescription: $"SettingsGroup with 'GroupName' '{targetGroupName}' not found or does not have 'Items'.", mCloseButtonText: "Ok", mPrimaryButtonText: "Retry");
            }
        }
        catch (Exception ex) {
            // Handle any exceptions that might occur during the process
            await ContentDialogExtension.ShowDialogAsync(mTitle: "PowerCommander", mDescription: $"A problem has ocurred while trying to load JSON file {ex.Message}", mCloseButtonText: "Ok", mPrimaryButtonText: "");
        }
    }



    private async Task SearchForUniqueID(string mRegistryGroupName)
    {
        try {
            // Read the content from the JSON file containing settings items
            var settingsJsonContent = File.ReadAllText(mSettingsElements);

            // Deserialize the JSON into a list of SettingsGroups
            var settingsGroups = JsonConvert.DeserializeObject<List<SettingsGroups>>(settingsJsonContent);

            if (settingsGroups != null) {
                // Find the target UniqueID in the list of settings groups
                var mTargetUniqueID = settingsGroups
                    ?.SelectMany(group => group.Items ?? Enumerable.Empty<SettingsItem>())
                    .FirstOrDefault(item => item.UniqueID == mRegistryGroupName);

                if (mTargetUniqueID != null) {
                    // Check if there is at least one ToggleSwitch that is enabled
                    if (mTargetUniqueID.ToggleSwitchState) {
                        // After finding the UniqueID, apply the specific registry settings
                        await ApplyRegistrySettingsForUniqueID(mRegistryGroupName);
                    }
                }
                else {
                    // Handle the case where the target UniqueID is not found
                    // You might want to display a message or take other appropriate actions
                }
            }
        }
        catch (Exception ex) {
            // Handle any exceptions that might occur during the process
            await ContentDialogExtension.ShowDialogAsync(mTitle: "PowerCommander", mDescription: $"A problem has occurred while trying to load JSON file {ex.Message}", mCloseButtonText: "Ok", mPrimaryButtonText: "");
        }
    }


    /// <summary>
    /// Applies registry settings based on the information provided in a JSON file for a specific UniqueID.
    /// </summary>
    /// <param name="mRegistryGroupName">UniqueID used to filter registry settings.</param>
    private async Task ApplyRegistrySettingsForUniqueID(string mRegistryGroupName)
    {
        try {
            // Read the content from the JSON file of the registry
            var registryJsonContent = File.ReadAllText(mRegistry);

            // Deserialize the JSON into a list of RegistrySettings
            var registrySettings = JsonConvert.DeserializeObject<List<RegistrySettings>>(registryJsonContent);

            // Filter the list of RegistrySettings to get only those with RegistryGroupName equal to the UniqueID
            var filteredRegistrySettings = registrySettings?
                .Where(registrySetting => registrySetting.RegistryGroupName == mRegistryGroupName)
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

                                // Create the new key with the specified value
                                if (item.Type == "DWORD") {
                                    registryKey.SetValue(item.Name, int.Parse(item?.Keyvalue?.EnableValue!), RegistryValueKind.DWord);
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

    /// <summary>
    /// Counts the total number of 'Items' across all SettingsGroups in the JSON file.
    /// </summary>
    /// <returns>The total count of 'Items' in all groups.</returns>
    private async Task<int> CountTotalItems(int mTotalCount)
    {
        try {
            // Read the content from the JSON file
            var jsonContent = File.ReadAllText(mSettingsElements);

            // Deserialize the JSON into a list of SettingsGroups
            var settingsGroups = JsonConvert.DeserializeObject<List<SettingsGroups>>(value: jsonContent);

            // Iterate through each SettingsGroup
            foreach (var group in settingsGroups!) {
                // Check if the current group has 'Items'
                if (group?.Items != null) {
                    // Increment the total count by the number of 'Items' in the current group
                    mTotalCount += group.Items.Count;
                }
            }

            // Return the total count of 'Items' in all groups
            return mTotalCount;
        }
        catch (Exception ex) {
            // Handle any exceptions that might occur during the process
            await ContentDialogExtension.ShowDialogAsync(mTitle: "PowerCommander", mDescription: $"A problem has occurred while trying to Count the items in the JSON file {ex.Message}", mCloseButtonText: "Ok", mPrimaryButtonText: "");

            // Return 0 if there was an issue
            return 0;
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

        // Execute the command to retrieve the user's profile picture asynchronously.
        GetUserProfileInfoCommand.Execute(this);

        // Update the PlaceHolderSuggestions property with a message indicating the search for the total number of tweaks.
        PlaceHolderSuggestions = $"Search for {await CountTotalItems(TotalTweaks)} Tweaks...";
    }

    [RelayCommand]
    public async Task ExecuteRegistryTask()
    {
        var mRegistryPath = File.ReadAllText(mRegistry);

        // Deserialize the JSON into a list of SettingsGroups
        var mRegistryData = JsonConvert.DeserializeObject<List<RegistrySettings>>(value: mRegistryPath);

        // For every Regedit found in registryData...
        foreach (var regedit in mRegistryData!) {

            // Add Search and Add for the unique ID
            await SearchForUniqueID(regedit.RegistryGroupName!);
        }
    }

    /// <summary>
    /// Command to retrieve the user's profile information including picture, email, and account name.
    /// </summary>
    [RelayCommand]
    private async Task GetUserProfileInfo()
    {
        try {
            // Find the local user and retrieve their profile information.
            var user = (await User.FindAllAsync(UserType.LocalUser)).FirstOrDefault();
            if (user != null) {

                // Get the user's picture
                var userPicture = await user.GetPictureAsync(UserPictureSize.Size64x64);

                // Add the first name to the Username string
                Username = (await user.GetPropertyAsync(KnownUserProperties.DisplayName))?.ToString()?.ToUpper();

                // If Username is Empty...
                if (string.IsNullOrEmpty(Username))
                    // Use the local machine name.
                    Username = Environment.UserName;

                // Add the last name to the Email string
                Email = (await user.GetPropertyAsync(KnownUserProperties.AccountName))?.ToString();

                // If Email is null...
                if (string.IsNullOrEmpty(Email))
                    // Set it to "Local Account" to indicate that the user's email is not available.
                    Email = "Local Account";

                // If a profile picture is available, set it as the system picture.
                if (userPicture != null) {
                    // Open the picture stream and create a BitmapImage from it.
                    using var stream = await userPicture.OpenReadAsync();
                    var bitmapImage = new BitmapImage();
                    await bitmapImage.SetSourceAsync(stream);

                    // Set the system picture to the retrieved BitmapImage.
                    AccountPicture = bitmapImage;
                }
            }
        }
        catch (Exception ex) {
            // Handle any exceptions that might occur during the process
            await ContentDialogExtension.ShowDialogAsync(mTitle: "PowerCommander", mDescription: $"A problem has occurred while trying to retrieve user profile information: {ex.Message}", mCloseButtonText: "Ok", mPrimaryButtonText: "");
        }
    }


    #endregion
}
