namespace TownOfUsReworked.Managers;

public static class ReworkedDataManager
{
    public static void Setup()
    {
        var path = Path.Combine(PlatformPaths.persistentDataPath, "reworkedData");

        if (File.Exists(path))
        {
            try
            {
                using var reader = new BinaryReader(File.OpenRead(path));
                reader.DeserializeReworkedData();
            } catch {}
        }


        CustomStatsManager.Setup();
        CustomAchievementManager.Setup();

        StatsManager.Instance.LoadStats(); // Forcing stat loading to ensure proper stats are loaded
        StatsManager.Instance.SaveStats(); // Force save the stats to save any new achievements and stats
    }

    public static void DeserializeReworkedData(this BinaryReader reader)
    {
        TranslationManager.PreviousLastID = reader.ReadInt32();
    }

    public static void SerializeReworkedData(this BinaryWriter writer)
    {
        writer.Write(TranslationManager.LastID);
    }
}