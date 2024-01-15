using System.Reflection;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.UI.Xaml;

using PowerCommander.Contracts.Services;
using PowerCommander.Helpers;

using Windows.ApplicationModel;

namespace PowerCommander.ViewModels;

public partial class SettingsViewModel : ObservableRecipient
{
    #region Private Const properties

    /// <summary>
    /// Service used for navigation purposes.
    /// </summary>
    private readonly INavigationService _navigationService;

    /// <summary>
    /// Service used for changing theme in runtime.
    /// </summary>
    private readonly IThemeSelectorService _themeSelectorService;

    #endregion

    #region Public properrties
    [ObservableProperty]
    private ElementTheme _elementTheme;

    #endregion

    #region Commands
    public ICommand SwitchThemeCommand
    {
        get;
    }
    #endregion

    /// <summary>
    /// Main Constructor
    /// </summary>
    /// <param name="themeSelectorService">Service for changing the theme dynamically</param>
    /// <param name="navigationService">Service for navigation purposes</param>
    public SettingsViewModel(IThemeSelectorService themeSelectorService, INavigationService navigationService)
    {
        // Initialize themeSelectorService and other properties
        _themeSelectorService = themeSelectorService;
        _elementTheme = _themeSelectorService.Theme;
        _navigationService = navigationService;

        // Initialize the SwitchThemeCommand with RelayCommand
        SwitchThemeCommand = new RelayCommand<ElementTheme>(
            async (param) => {
                // Switch theme only if the selected theme is different
                if (ElementTheme != param) {
                    ElementTheme = param;
                    await _themeSelectorService.SetThemeAsync(param);
                }
            });
    }

    #region private members

    /// <summary>
    /// Navigate to MainPage
    /// </summary>
    [RelayCommand]
    private void NavigateToMainPage() =>
        _navigationService.NavigateTo(typeof(MainViewModel).FullName!);


    #endregion
}
