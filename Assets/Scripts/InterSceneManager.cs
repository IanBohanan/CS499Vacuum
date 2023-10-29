public static class InterSceneManager
{
    // Main Menu Import File:
    public static string fileSelection = "";

    // House Builder UI -> ClickDrop
    public static bool deleteMode = false;

    // Simulation Setup Settings:
    private static bool whiskersEnabled;
    private static string floorCovering;
    private static int batteryLife;
    private static bool randomAlgEnabled;
    private static bool spiralAlgEnabled;
    private static bool snakingAlgEnabled;
    private static bool wallFollowAlgEnabled;

    public static void setSimulationSettings(bool whiskers, string floorCov, int battery, bool randomAlg, bool spiralAlg, bool snakingAlg, bool wallFollowAlg)
    {
        whiskersEnabled = whiskers;
        floorCovering = floorCov;
        batteryLife = battery;
        randomAlgEnabled = randomAlg;
        spiralAlgEnabled = spiralAlg;
        snakingAlgEnabled = snakingAlg;
        wallFollowAlgEnabled = wallFollowAlg;
        return;
    }

    public static (bool, string, int, bool, bool, bool, bool) getSimulationSettings() => (whiskersEnabled, floorCovering, batteryLife, randomAlgEnabled, spiralAlgEnabled, snakingAlgEnabled, wallFollowAlgEnabled);

    private static int batteryLifeMinutes = 150;  // default

    public static void SetBatteryLifeMinutes(int value) => batteryLifeMinutes = value;

    public static int GetBatteryLifeMinutes() => batteryLifeMinutes;

    // ... other existing methods and properties
}