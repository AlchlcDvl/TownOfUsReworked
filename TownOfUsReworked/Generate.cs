namespace TownOfUsReworked;

/// <summary>
/// Initialises and generates all necessary objects and data within the mod.
/// </summary>
public static partial class Generate
{
    /// <summary>
    /// Master methods that generates everything mod related. Called only once and after the game's intro splash.
    /// </summary>
    public static void GenerateAll()
    {
        GenerateOptions();
        GenerateIDs();
    }
}