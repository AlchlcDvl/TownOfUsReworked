namespace TownOfUsReworked.Modules;

// Taken from Submerged going open source recently
public readonly struct KillAnimFrame(int index, float time, int length, Vector2 offset)
{
    public readonly int Animation = index;
    public readonly float Time = time;
    public readonly int Length = length;
    public readonly Vector2 Offset = offset;

    // public static string Serialize(KillAnimFrame frame) => $"{frame.Animation},{frame.Time},{frame.Length},{frame.Offset.x},{frame.Offset.y}";

    public static KillAnimFrame Deserialize(string dataString)
    {
        var data = dataString.TrueSplit(',');
        return new(int.Parse(data[0]), float.Parse(data[1]), int.Parse(data[2]), new(float.Parse(data[3]), float.Parse(data[4])));
    }
}