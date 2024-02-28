namespace PowerCommander.Helpers;


public static class DefaultToggleSwitchStates
{
    public static Dictionary<string, bool> DefaultStates = new()
    {
        { "UAC", true },
        { "Telemetry", true },
        { "FirewallSettings", false },
        { "AntivirusConfiguration", true },
        { "CoreIsolation", false },
        { "Hibernation", true },
        { "PowerThrottling", false },
        { "NetworkThrottling", true },
        { "PowerPlanIDLE", true },
        { "Sleep", true },
        { "DriverSearching", false },
        { "SuggestedActions", false },
        { "MouseAndKeyboard", false },
        { "BackgroundApps", true },
        { "FullScreenExclusive", true },
        { "P0State", true },
        { "HDCP", true },
        { "NvidiaSharpening", false },
        { "NvidiaTrayIcon", true },
        { "Preemption", true },
        { "PCIeLinkSpeed", true },
        { "WindowsPrioritySeparation", true },
        { "QualityofService", false },
        { "LanmanServiceSettings", true },
        { "NetworkService", true },
    };

    /// <summary>
    /// Gets the default state for a toggle switch based on the provided unique ID.
    /// </summary>
    /// <param name="uniqueID">The unique identifier associated with the toggle switch.</param>
    /// <returns>
    /// The default state for the toggle switch. If the unique ID is found in the dictionary,
    /// the corresponding default state is returned; otherwise, the default value of 'false' is returned.
    /// </returns>
    public static bool GetDefaultState(string uniqueID)
    {
        // Attempt to retrieve the default state from the dictionary based on the unique ID
        if (DefaultStates.TryGetValue(uniqueID, out bool defaultState)) {
            // Return the retrieved default state if the unique ID is found in the dictionary
            return defaultState;
        }

        // If UniqueID was not found, then set it to false.
        return false;
    }

}
