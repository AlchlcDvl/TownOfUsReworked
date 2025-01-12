namespace TownOfUsReworked.Modules;

// Taken from Submerged going open source recently
public struct KillAnimFrame
{
    public int Animation { get; set; }
    public float Time { get; set; }
    public int Length { get; set; }
    public Vector2 Offset { get; set; }

    public static string Serialize(KillAnimFrame frame) => $"{frame.Animation},{frame.Time},{frame.Length},{frame.Offset.x},{frame.Offset.y}";

    public static KillAnimFrame Deserialize(string dataString)
    {
        var data = dataString.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return new()
        {
            Animation = int.Parse(data[0]),
            Time = float.Parse(data[1]),
            Length = int.Parse(data[2]),
            Offset = new(float.Parse(data[3]), float.Parse(data[4]))
        };
    }
}