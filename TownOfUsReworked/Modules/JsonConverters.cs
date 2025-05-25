namespace TownOfUsReworked.Modules;

public sealed class Vector2JsonConverter : JsonConverter<Vector2>
{
    public override Vector2 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var parts = reader.GetString().TrueSplit(',');
        return new(float.Parse(parts[0]), float.Parse(parts[1]));
    }

    public override void Write(Utf8JsonWriter writer, Vector2 value, JsonSerializerOptions options) => writer.WriteStringValue($"{value.x},{value.y}");
}