using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace PowerCommander.Helpers;

public static class ContentDialogExtension
{
    /// <summary>
    /// A flag indicating where the ContentDialog should be displayed.
    /// </summary>
    private static XamlRoot? mXamlRoot;

    /// <summary>
    /// Initializes the ContentDialogExtension with the specified XamlRoot.
    /// </summary>
    /// <param name="xamlRoot">The XamlRoot to be used for displaying ContentDialogs.</param>
    public static void Initialize(XamlRoot xamlRoot) =>
        mXamlRoot = xamlRoot;

    /// <summary>
    /// Shows a ContentDialog asynchronously with the provided title, description, and button options.
    /// </summary>
    /// <param name="mTitle">The title of the ContentDialog.</param>
    /// <param name="mDescription">The description or content of the ContentDialog.</param>
    /// <param name="mCloseButtonText">The text for the close button of the ContentDialog.</param>
    /// <param name="mPrimaryButtonText">The text for the primary button of the ContentDialog (nullable).</param>
    /// <returns>A task representing the asynchronous operation and returning the result of the ContentDialog.</returns>
    public static async Task<ContentDialogResult> ShowDialogAsync(string? mTitle, string? mDescription, string? mCloseButtonText, string? mPrimaryButtonText)
    {
        // Create a ContentDialog and populate it with the provided data
        ContentDialog mContentDialog = new() {
            XamlRoot = mXamlRoot,
            Title = mTitle,
            Content = mDescription,
            PrimaryButtonText = mPrimaryButtonText,
            CloseButtonText = mCloseButtonText,
        };

        // Display the ContentDialog asynchronously and return the result
        return await mContentDialog.ShowAsync();
    }
}
