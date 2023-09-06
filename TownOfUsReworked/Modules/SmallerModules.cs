namespace TownOfUsReworked.Modules;

public class GenerationData
{
    public int Chance;
    public LayerEnum ID;
    public bool Unique;

    public GenerationData(int chance, LayerEnum id, bool unique)
    {
        Chance = chance;
        ID = id;
        Unique = unique;
    }
}

public class PlayerInfo
{
    public readonly string PlayerName;
    public readonly string History;
    public readonly string CachedHistory;

    public PlayerInfo(string name, string history, string cache)
    {
        PlayerName = name;
        History = history;
        CachedHistory = cache;
    }
}

public class PlayerVersion
{
    public readonly Version Version;
    public readonly Guid Guid;
    public bool GuidMatches => TownOfUsReworked.Core.ManifestModule.ModuleVersionId.Equals(Guid);

    public PlayerVersion(Version version, Guid guid)
    {
        Version = version;
        Guid = guid;
    }
}

public class PointInTime
{
    public readonly Vector3 Position;

    public PointInTime(Vector3 position) => Position = position;
}