namespace TownOfUsReworked.Managers;

public static class ReworkedDataManager
{
    // public static int VisorColorId = 3;

    public static void Setup()
    {
        // Loading the custom cosmetic data
        // var path = Path.Combine(Application.persistentDataPath, "reworkedData");

        // if (File.Exists(path))
        // {
        //     try
        //     {
        //         using var reader = new BinaryReader(File.OpenRead(path));
        //         reader.DeserializeReworkedData();
        //     } catch {}
        // }

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

        // try
        // {
        //     using var writer = new BinaryWriter(File.OpenWrite(Path.Combine(Application.persistentDataPath, "reworkedData")));
        //     writer.SerializeReworkedData();
        // }
        // catch (Exception ex)
        // {
        //     Error($"Failed to write out reworked data: {ex}");
        // }
    }

    // private static void SerializeReworkedData(this BinaryWriter writer) => writer.Write(VisorColorId);

    // private static void DeserializeReworkedData(this BinaryReader reader) => VisorColorId = reader.ReadInt32();
}