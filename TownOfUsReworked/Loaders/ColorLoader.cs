using static TownOfUsReworked.Managers.CustomColorManager;

namespace TownOfUsReworked.Loaders;

public class ColorLoader : AssetLoader<CustomColor>
{
    public override string DirectoryInfo => TownOfUsReworked.Colors;
    public override string Manifest => "Colors";

    public static ColorLoader Instance { get; set; }

    public override IEnumerator AfterLoading(CustomColor[] response)
    {
        var colors = new List<CustomColor>(response);

        if (TownOfUsReworked.IsStream)
        {
            var filePath = Path.Combine(DirectoryInfo, "Stream", "Colors.json");

            if (File.Exists(filePath))
            {
                var data = JsonSerializer.Deserialize<CustomColor[]>(File.ReadAllText(filePath));
                data.ForEach(x => x.StreamOnly = true);
                colors.AddRange(data);
                Array.Clear(data);
            }
        }

        colors.RemoveAll(x => x.StreamOnly && !TownOfUsReworked.IsStream);
        Message($"Found {colors.Count} colors");
        var time = 0f;

        for (var i = 0; i < colors.Count; i++)
        {
            var color = colors[i];
            color.ColorID = i;

            if (color.MainColorValues != null)
                color.MainColors = [ .. color.MainColorValues.Select(FromHex) ];

            if (color.ShadowColorValues != null)
                color.ShadowColors = [ .. color.ShadowColorValues.Select(FromHex) ];

            color.Changing = color.MainColorValues?.Length > 1;
            color.TimeSpeed = color.TimeSpeed == 0f ? 1f : color.TimeSpeed;

            if (!color.Default)
                color.StringID = 999999 - color.ColorID;

            AllColors[i] = color;
            time += Time.deltaTime;

            if (time > 1f)
            {
                time = 0f;
                UpdateSplashPatch.SetText($"Loading Colors ({i + 1}/{colors.Count})");
                yield return EndFrame();
            }
        }

        Palette.ColorNames = colors.Select(x => (StringNames)x.StringID).ToArray();
        Palette.PlayerColors = colors.Select(x => (Color32)x.GetColor()).ToArray();
        Palette.ShadowColors = colors.Select(x => (Color32)x.GetShadowColor()).ToArray();
        Palette.TextOutlineColors = Palette.PlayerColors.Select(x => x.Alternate()).ToArray();
        Palette.TextColors = Palette.PlayerColors;
        colors.Clear();
        yield break;
    }
}