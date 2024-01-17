using Microsoft.UI;
using Microsoft.UI.Windowing;
using PowerCommander.Helpers;
using Windows.UI.ViewManagement;
using WinUIEx;

namespace PowerCommander;

public sealed partial class MainWindow : WindowEx
{
    #region Private Properties

    /// <summary>
    /// Dispatcher used for changing the theme at runtime.
    /// </summary>
    private readonly Microsoft.UI.Dispatching.DispatcherQueue _dispatcherQueue;

    /// <summary>
    /// Common application user settings.
    /// </summary>
    private readonly UISettings _settings;

    #endregion

    /// <summary>
    /// Main constructor for the MainWindow class.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();

        // Set window icon
        AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets/WindowIcon.ico"));

        // Set content to null
        Content = null;

        // Set window title
        Title = "AppDisplayName".GetLocalized();

        // Theme change code adapted from https://github.com/microsoft/WinUI-Gallery/pull/1239
        _dispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();
        _settings = new UISettings();
        _settings.ColorValuesChanged += Settings_ColorValuesChanged; // Unable to use FrameworkElement.ActualThemeChanged event

        // Apply custom style to the mainWindow(appHeight, appWidth)
        SetWindowProperties(800, 576);

    }

    /// <summary>
    /// Sets various properties of the window, such as size and behavior.
    /// </summary>
    /// <param name="appHeight">The desired height of the window.</param>
    /// <param name="appWidth">The desired width of the window.</param>
    private void SetWindowProperties(int appHeight, int appWidth)
    {
        // Get hwnd from the current window
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);

        // Get the ID from the view using the hwnd 
        var wndID = Win32Interop.GetWindowIdFromWindow(hwnd);

        // Get the current window properties
        var appWindow = AppWindow.GetFromWindowId(wndID);

        // Resize the window
        appWindow.Resize(new Windows.Graphics.SizeInt32(appWidth, appHeight));

        // Overlap the current presenter
        var presenter = appWindow.Presenter as OverlappedPresenter;

        // if presenter is not null..
        if (presenter != null) {

            // Set window properties: IsMaximizable, IsMinimizable, IsResizable, IsAlwaysOnTop
            presenter.IsMaximizable = false;
            presenter.IsMinimizable = false;
            presenter.IsResizable = false;
            presenter.IsAlwaysOnTop = false;

        }
    }

    /// <summary>
    /// Handles updating the caption button colors correctly when the Windows system theme is changed
    /// while the app is open.
    /// </summary>
    /// <param name="sender">The sender object.</param>
    /// <param name="args">The event arguments.</param>
    private void Settings_ColorValuesChanged(UISettings sender, object args) =>
        // This call comes off-thread; hence, we need to dispatch it to the current app's thread
        _dispatcherQueue.TryEnqueue(TitleBarHelper.ApplySystemThemeToCaptionButtons);
}
