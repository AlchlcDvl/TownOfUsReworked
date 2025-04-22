namespace TownOfUsReworked.Managers;

public static class ReworkedDataManager
{
    public static void Setup()
    {
        CustomStatsManager.Setup();
        CustomAchievementManager.Setup();

        Save(); // Ensuring the data files are up-to-date
    }

    public static void Save()
    {
        try
        {
            using var writer = new BinaryWriter(File.OpenWrite(Path.Combine(Application.persistentDataPath, "reworkedStats")));
            writer.SerializeCustomStats();
        }
        catch (Exception ex)
        {
            Error($"Failed to write out reworked stats: {ex}");
        }

        try
        {
            using var writer = new BinaryWriter(File.OpenWrite(Path.Combine(Application.persistentDataPath, "reworkedAchievements")));
            writer.SerializeCustomAchievements();
        }
        catch (Exception ex)
        {
            Error($"Failed to write out reworked achievements: {ex}");
        }
    }
}