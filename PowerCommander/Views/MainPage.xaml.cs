using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PowerCommander.Helpers;
using PowerCommander.ViewModels;
using Windows.UI.Core;

namespace PowerCommander.Views;

public sealed partial class MainPage : Page
{
    /// <summary>
    /// MainViewModel
    /// </summary>
    public MainViewModel ViewModel { get; }

    /// <summary>
    /// Main Constructor
    /// </summary>
    public MainPage()
    {
        // InitializeComponent
        InitializeComponent();

        // Attach the event handler to the Loaded event of the RootGrid.
        RootGrid.Loaded += RootGrid_Loaded;

        // ViewModel Service
        ViewModel = App.GetService<MainViewModel>();

        // Extend content into titlebar
        App.MainWindow.ExtendsContentIntoTitleBar = true;

        // Set Custom TitleBar
        App.MainWindow.SetTitleBar(AppTitleBar);

        // Start the asynchronous execution of the ViewModel's initialization command for the page.
        ViewModel.InitializeViewModelAsyncCommand.Execute(this);
    }

    /// <summary>
    /// Event handler called when the RootGrid is loaded.
    /// </summary>
    /// <param name="sender">The object that triggered the event.</param>
    /// <param name="e">The event arguments.</param>
    private void RootGrid_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) =>
        // Initialize the ContentDialogExtension with the XamlRoot of the RootGrid.
        ContentDialogExtension.Initialize(this.RootGrid.XamlRoot);

}
