using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;

using PowerCommander.Activation;
using PowerCommander.Contracts.Services;
using PowerCommander.Core.Contracts.Services;
using PowerCommander.Core.Services;
using PowerCommander.Models;
using PowerCommander.Services;
using PowerCommander.ViewModels;
using PowerCommander.Views;
using System.Diagnostics;
using WinUIEx;

namespace PowerCommander;

// To learn more about WinUI 3, see https://docs.microsoft.com/windows/apps/winui/winui3/.
public partial class App : Application
{
    // The .NET Generic Host provides dependency injection, configuration, logging, and other services.
    // https://docs.microsoft.com/dotnet/core/extensions/generic-host
    // https://docs.microsoft.com/dotnet/core/extensions/dependency-injection
    // https://docs.microsoft.com/dotnet/core/extensions/configuration
    // https://docs.microsoft.com/dotnet/core/extensions/logging
    public IHost Host
    {
        get;
    }

    public static T GetService<T>()
        where T : class
    {
        if ((App.Current as App)!.Host.Services.GetService(typeof(T)) is not T service) {
            throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.");
        }

        return service;
    }

    public static WindowEx MainWindow { get; } = new MainWindow();

    public static UIElement? AppTitlebar { get; set; }

    public App()
    {
        InitializeComponent();

        Host=Microsoft.Extensions.Hosting.Host.
        CreateDefaultBuilder().
        UseContentRoot(AppContext.BaseDirectory).
        ConfigureServices((context, services) => {
            // Default Activation Handler
            services.AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>();

            // Other Activation Handlers

            // Services
            services.AddSingleton<ILocalSettingsService, LocalSettingsService>();
            services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
            services.AddSingleton<IActivationService, ActivationService>();
            services.AddSingleton<IPageService, PageService>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IFetchJSONDataService, FetchJSONDataService>();

            // Core Services
            services.AddSingleton<IFileService, FileService>();

            // Views and ViewModels
            services.AddTransient<SettingsViewModel>();
            services.AddTransient<SettingsPage>();
            services.AddTransient<MainViewModel>();
            services.AddTransient<MainPage>();

            // Configuration
            services.Configure<LocalSettingsOptions>(context.Configuration.GetSection(nameof(LocalSettingsOptions)));
        }).
        Build();

        UnhandledException+=App_UnhandledException;
    }

    private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        // Log the exception
        LogException(e.Exception);

        // Mark the exception as handled to prevent the application from crashing
        e.Handled=true;
    }

    /// <summary>
    /// Logs an unhandled exception to a file on the desktop. If the specified directory
    /// ("PowerCommanderException") doesn't exist, it creates the directory before
    /// appending the exception details to the log file.
    /// </summary>
    /// <param name="exception">The exception to be logged.</param>
    private static void LogException(Exception exception)
    {
        // Get Desktop Path
        var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

        // Combine the desktop path with the new folder and file names
        var logFolderPath = Path.Combine(desktopPath, "PowerCommanderException");
        var logFilePath = Path.Combine(logFolderPath, "Exception.txt");

        // Create the directory if it doesn't exist
        if (!Directory.Exists(logFolderPath)) {
            Directory.CreateDirectory(logFolderPath);
        }

        // Appending to the log file
        File.AppendAllText(logFilePath, $"Unhandled Exception: {exception.Message}{Environment.NewLine}");

    }

    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);

        await App.GetService<IActivationService>().ActivateAsync(args);
    }
}
