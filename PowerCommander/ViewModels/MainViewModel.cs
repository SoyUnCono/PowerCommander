using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
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
    /// Service used for navigation purposes.
    /// </summary>
    private readonly Contracts.Services.INavigationService _navigationService;

    /// <summary>
    /// Service used for Fetching Data.
    /// </summary>
    private readonly Contracts.Services.IFetchJSONDataService _fetchJSONDataService;

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
    /// Indicates if the current thread is busy or not
    /// </summary>
    [ObservableProperty][NotifyPropertyChangedFor(nameof(IsNotBusy))] private bool _isBusy;

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
    /// Set the value to the oposite of IsBusy to update the UI
    /// </summary>
    public bool IsNotBusy => !IsBusy;

    /// <summary>
    /// Main constructor for the MainViewModel class.
    /// </summary>
    public MainViewModel(Contracts.Services.INavigationService navigationService, Contracts.Services.IFetchJSONDataService fetchJSONDataService)
    {
        // Initialize navigationService
        _navigationService=navigationService;

        // Initialize fetching service
        _fetchJSONDataService=fetchJSONDataService;
    }


    #region private members

    /// <summary>
    /// Counts the total number of 'Items' across all SettingsGroups in the JSON file.
    /// </summary>
    /// <returns>The total count of 'Items' in all groups.</returns>
    private static async Task<int> CountTotalItems(int totalCount)
    {
        using (HttpClient client = new()) {

            try {
                // Read the content from the JSON file
                var jsonContent = await client.GetStringAsync(Constants.UriConstants.SettingsElementsURL);

                // Deserialize the JSON into a list of SettingsGroups
                var settingsGroups = JsonConvert.DeserializeObject<List<SettingsGroups>>(value: jsonContent);

                // Iterate through each SettingsGroup
                foreach (var group in settingsGroups!) {
                    // Check if the current group has 'Items'
                    if (group?.Items!=null) {
                        // Increment the total count by the number of 'Items' in the current group
                        totalCount+=group.Items.Count;
                    }
                }

                // Return the total count of 'Items' in all groups
                return totalCount;
            }
            catch (Exception ex) {
                // Handle any exceptions that might occur during the process
                await ContentDialogExtension.ShowDialogAsync(mTitle: "PowerCommander", mDescription: $"A problem has occurred while trying to Count the items in the JSON file {ex.Message}", mCloseButtonText: "Ok", mPrimaryButtonText: "");

                // Return 0 if there was an issue
                return 0;
            }
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
    private async Task InitializeViewModelAsync()
    {
        IsBusy=true;
        // Initialize the SettingsCard lists
        SecurityAndPrivacy=await _fetchJSONDataService.InyectDataToList("SecurityAndPrivacy");
        Power=await _fetchJSONDataService.InyectDataToList("Power");
        GamemodeTweaks=await _fetchJSONDataService.InyectDataToList("Gamemode");
        OtherTweak=await _fetchJSONDataService.InyectDataToList("OtherTweaks");
        Network=await _fetchJSONDataService.InyectDataToList("Network");

        IsBusy=false;
        // Execute the command to retrieve the user's profile picture asynchronously.
        GetUserProfileInfoCommand.Execute(this);

        // Update the PlaceHolderSuggestions property with a message indicating the search for the total number of tweaks.
        PlaceHolderSuggestions=$"Search for {await CountTotalItems(TotalTweaks)} Tweaks...";
    }


    /// <summary>
    /// Executes a registry task asynchronously using a RelayCommand.
    /// </summary>
    /// <returns>A Task representing the asynchronous operation.</returns>
    [RelayCommand]
    public async Task ExecuteRegistryTask()
    {
        // Using statement ensures proper disposal of HttpClient
        using (HttpClient client = new()) {

            // Fetch the RegistryElementsURL content as a string asynchronously
            var regeditFilePath = await client.GetStringAsync(Constants.UriConstants.RegistryElementsURL);

            // Deserialize the JSON string into a list of RegistrySettings objects
            var mRegistryData = JsonConvert.DeserializeObject<List<RegistrySettings>>(value: regeditFilePath);

            // Read the content from the JSON file containing settings items
            var settingsJsonContent = await client.GetStringAsync(Constants.UriConstants.SettingsElementsURL);

            // Deserialize the JSON into a list of SettingsGroups
            var settingsGroups = JsonConvert.DeserializeObject<List<SettingsGroups>>(settingsJsonContent);

            // Itera a través de cada objeto RegistrySettings en la lista
            foreach (var regedit in mRegistryData!) {
                // Busca el UniqueID correspondiente en la lista de grupos de configuración
                var targetUniqueID = settingsGroups!
                    .SelectMany(group => group.Items!)
                    .FirstOrDefault(item => item.UniqueID==regedit.RegistryGroupName);

                // Verifica si se encuentra al menos un ToggleSwitch habilitado
                if (targetUniqueID!.ToggleSwitchState==true) {
                    // Después de encontrar el UniqueID, aplica la configuración específica del registro
                    await _fetchJSONDataService.ApplyRegistrySettingsForUniqueID(regedit.RegistryGroupName!);
                }
            }

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
            if (user!=null) {

                // Get the user's picture
                var userPicture = await user.GetPictureAsync(UserPictureSize.Size64x64);

                // Add the first name to the Username string
                Username=(await user.GetPropertyAsync(KnownUserProperties.DisplayName))?.ToString()?.ToUpper();

                // If Username is Empty...
                if (string.IsNullOrEmpty(Username))
                    // Use the local machine name.
                    Username=Environment.UserName;

                // Add the last name to the Email string
                Email=(await user.GetPropertyAsync(KnownUserProperties.AccountName))?.ToString();

                // If Email is null...
                if (string.IsNullOrEmpty(Email))
                    // Set it to "Local Account" to indicate that the user's email is not available.
                    Email="Local Account";

                // If a profile picture is available, set it as the system picture.
                if (userPicture!=null) {
                    // Open the picture stream and create a BitmapImage from it.
                    using var stream = await userPicture.OpenReadAsync();
                    var bitmapImage = new BitmapImage();
                    await bitmapImage.SetSourceAsync(stream);

                    // Set the system picture to the retrieved BitmapImage.
                    AccountPicture=bitmapImage;
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
