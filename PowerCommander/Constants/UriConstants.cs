namespace PowerCommander.Constants;

/// <summary>
/// 
/// </summary>
public static class UriConstants
{
    private static string baseURL = "https://gist.githubusercontent.com/SoyUnCono/";

    private static string settingsRawURL = "1e4d70b6eb365ff8253755dac152b4b1/raw/843aae9a243df089aeb5e200e11f4b3a1cf187ae";

    private static string regeditRawURL = "1e4d70b6eb365ff8253755dac152b4b1/raw/843aae9a243df089aeb5e200e11f4b3a1cf187ae";

    /// <summary>
    /// Path to a JSON file with all the data needed.
    /// </summary>
    public static string SettingsElementsURL = $"{baseURL}/{settingsRawURL}/SettingsElements.json";

    /// <summary>
    /// Path to a JSON file with Regedits based on UniquedID's.
    /// </summary>
    public static string RegistryElementsURL = $"{baseURL}/{regeditRawURL}/Regedit.json";
}
