using static TownOfUsReworked.Cosmetics.CustomColors.CustomColorManager;
using System.Text.Json;

namespace TownOfUsReworked.Monos;

public class ColorsLoader : AssetLoader
{
    public override string DirectoryInfo => TownOfUsReworked.Colors;
    public override string ManifestFileName => "Colors";

    [HideFromIl2Cpp]
    public override Type JSONType => typeof(ColorsJSON);

    public static ColorsLoader Instance { get; private set; }

    public ColorsLoader(IntPtr ptr) : base(ptr)
    {
        if (Instance)
            Instance.Destroy();

        Instance = this;
    }

    [HideFromIl2Cpp]
    public override void AfterLoading(object response)
    {
        var colors = (ColorsJSON)response;
        AllColors.AddRange(colors.Colors);
        var cache = AllColors.Count;
        LogMessage($"Found {cache} colors");

        if (TownOfUsReworked.IsStream)
        {
            var filePath = Path.Combine(DirectoryInfo, "Stream", "Colors.json");

            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                var data = JsonSerializer.Deserialize<ColorsJSON>(json, new JsonSerializerOptions() { AllowTrailingCommas = true });
                data.Colors.ForEach(x => x.StreamOnly = true);
                AllColors.AddRange(data.Colors);
            }
        }

        LogMessage($"Found {AllColors.Count - cache} local colors");
        AllColors.RemoveAll(x => x.StreamOnly && !TownOfUsReworked.IsStream);

        foreach (var color in AllColors)
        {
            color.ColorID = AllColors.IndexOf(color);
            color.Title ??= (color.Default ? "Innersloth" : "Custom");

            if (!color.Default)
                color.StringID = 999999 - color.ColorID;
        }

        Palette.ColorNames = AllColors.Select(x => (StringNames)x.StringID).ToArray();
        Palette.PlayerColors = AllColors.Select(x => x.MainColor).ToArray();
        Palette.ShadowColors = AllColors.Select(x => x.ShadowColor).ToArray();
        LogMessage($"Set {AllColors.Count} colors");
    }
}