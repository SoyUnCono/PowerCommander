using Microsoft.UI.Xaml.Controls;
using PowerCommander.ViewModels;

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
        // ViewModel Service
        ViewModel = App.GetService<MainViewModel>();

        // InitializeComponent
        InitializeComponent();

        // Extend content into titlebar
        App.MainWindow.ExtendsContentIntoTitleBar = true;

        // Set Custom TitleBar
        App.MainWindow.SetTitleBar(AppTitleBar);

        // Start the asynchronous execution of the ViewModel's initialization command for the page.
        Task.Run(() =>
            ViewModel.InitializeViewModelAsyncCommand.Execute(this));
    }
}
