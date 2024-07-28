using static TownOfUsReworked.Cosmetics.CustomColors.CustomColorManager;

namespace TownOfUsReworked.Loaders;

public class ColorLoader : AssetLoader<CustomColor>
{
    public override string DirectoryInfo => TownOfUsReworked.Colors;
    public override string Manifest => "Colors";

    public static ColorLoader Instance { get; set; }

    public override IEnumerator AfterLoading(object response)
    {
        var colors = (List<CustomColor>)response;
        AllColors.AddRange(colors);
        // LogMessage($"Found {AllColors.Count} colors");
        var cache = AllColors.Count;
        LogMessage($"Found {cache} colors");

        if (TownOfUsReworked.IsStream)
        {
            var filePath = Path.Combine(DirectoryInfo, "Stream", "Colors.json");

            if (File.Exists(filePath))
            {
                var data = JsonSerializer.Deserialize<List<CustomColor>>(File.ReadAllText(filePath));
                data.ForEach(x => x.StreamOnly = true);
                AllColors.AddRange(data);
            }
        }

        LogMessage($"Found {AllColors.Count - cache} local colors");
        AllColors.RemoveAll(x => x.StreamOnly && !TownOfUsReworked.IsStream);
        var time = 0f;

        for (var i = 0; i < AllColors.Count; i++)
        {
            var color = AllColors[i];
            color.ColorID = i;
            // color.Title ??= (color.Default ? "Innersloth" : "Custom");

            if (!color.Default)
                color.StringID = 999999 - color.ColorID;

            time += Time.deltaTime;

            if (time > 1f)
            {
                time = 0f;
                UpdateSplashPatch.SetText($"Loading Colors ({i + 1}/{AllColors.Count})");
                yield return EndFrame();
            }
        }

        Palette.ColorNames = AllColors.Select(x => (StringNames)x.StringID).ToArray();
        /*Palette.PlayerColors = AllColors.Select(x => x.MainColor).ToArray();
        Palette.ShadowColors = AllColors.Select(x => x.ShadowColor).ToArray();*/
        LoadColors();
        LogMessage($"Set {AllColors.Count} colors");
        colors.Clear();
        yield break;
    }
}