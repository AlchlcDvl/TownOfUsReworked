namespace TownOfUsReworked.Modules;

// Taken from Submerged going open source recently
public readonly struct KillAnimFrame
{
    public int Animation { get; private init; }
    public float Time { get; private init; }
    public int Length { get; private init; }
    public Vector2 Offset { get; private init; }

    // public static string Serialize(KillAnimFrame frame) => $"{frame.Animation},{frame.Time},{frame.Length},{frame.Offset.x},{frame.Offset.y}";

    public static KillAnimFrame Deserialize(string dataString)
    {
        var data = dataString.TrueSplit(',');
        return new()
        {
            Animation = int.Parse(data[0]),
            Time = float.Parse(data[1]),
            Length = int.Parse(data[2]),
            Offset = new(float.Parse(data[3]), float.Parse(data[4]))
        };
    }
}