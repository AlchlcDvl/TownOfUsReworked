namespace TownOfUsReworked.Cosmetics;

public static class CustomColors
{
    public static Color Rainbow => new HSBColor(PP(0f, 1f, 0.3f), 1f, 1f).ToColor();
    public static Color RainbowShadow => Shadow(Rainbow);

    public static Color Galaxy => new HSBColor(PP(0.5f, 0.87f, 0.4f), 1f, 1f).ToColor();
    public static Color GalaxyShadow => Shadow(Galaxy);

    public static Color Fire => new HSBColor(PP(0f, 0.17f, 0.4f), 1f, 1f).ToColor();
    public static Color FireShadow => Shadow(Fire);

    public static Color Monochrome => new HSBColor(1f, 0f, PP(0f, 1f, 0.8f)).ToColor();
    public static Color MonochromeShadow => Shadow(Monochrome);

    public static Color Mantle => new HSBColor(PP(0f, 1f, 0.3f), PP(0f, 1f, 0.9f), PP(0f, 0.8f, 0.5f)).ToColor();
    public static Color MantleShadow => Shadow(Mantle);

    public static Color Chroma => new HSBColor(PP(0f, 1f, 0.4f), PPR(0f, 1f, 0.6f), PP(0f, 1f, 0.9f)).ToColor();
    public static Color ChromaShadow => Shadow(Chroma);

    public static Color Reversebow => new HSBColor(PPR(0f, 1f, 0.3f), 1f, PPR(0f, 1f, 0.3f)).ToColor();
    public static Color ReversebowShadow => Shadow(Reversebow);

    public static Color Vibrance => new HSBColor(PPR(0.17f, 0.5f, 0.3f), PP(0.9f, 1f, 0.3f), PPR(0.9f, 1f, 0.3f)).ToColor();
    public static Color VibranceShadow => Shadow(Vibrance);

    public static Color Darkbow => new HSBColor(PP(0f, 1f, 0.3f), 0.8f, 0.3f).ToColor();
    public static Color DarkbowShadow => Shadow(Darkbow);

    public static Color Abberation => new HSBColor(PP(0f, 0.2f, 0.9f), PPR(0.8f, 1f, 0.3f), 0.3f).ToColor();
    public static Color AbberationShadow => Shadow(Abberation);

    public static float PP(float min, float max, float mul) => min + Mathf.PingPong(Time.time * mul, max - min);

    public static float PPR(float min, float max, float mul) => max - Mathf.PingPong(Time.time * mul, max - min);

    public static Color Shadow(Color color) => new(color.r - 0.2f < 0 ? 0f : color.r - 0.2f, color.g - 0.2f < 0 ? 0f : color.g - 0.2f, color.b - 0.2f < 0 ? 0f : color.b - 0.2f, color.a);

    public static void SetColor(Renderer rend, int id)
    {
        if (rend == null)
            return;

        rend.material.SetColor("_BackColor", GetColor(id, true));
        rend.material.SetColor("_BodyColor", GetColor(id, false));
        rend.material.SetColor("_VisorColor", Palette.VisorColor);
    }

    public static void SetChangingColor(Renderer rend, int id)
    {
        if (!IsChanging(id) || rend == null)
            return;

        rend.material.SetColor(PlayerMaterial.BackColor, GetColor(id, true));
        rend.material.SetColor(PlayerMaterial.BodyColor, GetColor(id, false));
        rend.material.SetColor(PlayerMaterial.VisorColor, Palette.VisorColor);
    }

    public static bool OutOfBounds(int id) => id < 0 || id >= Palette.ColorNames.Count;

    public static bool IsRainbow(int id) => !OutOfBounds(id) && (int)Palette.ColorNames[id] == 999999;

    public static bool IsMonochrome(int id) => !OutOfBounds(id) && (int)Palette.ColorNames[id] == 999998;

    public static bool IsGalaxy(int id) => !OutOfBounds(id) && (int)Palette.ColorNames[id] == 999997;

    public static bool IsFire(int id) => !OutOfBounds(id) && (int)Palette.ColorNames[id] == 999996;

    public static bool IsMantle(int id) => !OutOfBounds(id) && (int)Palette.ColorNames[id] == 999995;

    public static bool IsChroma(int id) => !OutOfBounds(id) && (int)Palette.ColorNames[id] == 999994;

    public static bool IsReversebow(int id) => !OutOfBounds(id) && (int)Palette.ColorNames[id] == 999993;

    public static bool IsVibrance(int id) => !OutOfBounds(id) && (int)Palette.ColorNames[id] == 999992;

    public static bool IsDarkbow(int id) => !OutOfBounds(id) && (int)Palette.ColorNames[id] == 999991;

    public static bool IsAbberration(int id) => !OutOfBounds(id) && (int)Palette.ColorNames[id] == 999990;

    public static bool IsChanging(int id) => IsRainbow(id) || IsFire(id) || IsGalaxy(id) || IsMantle(id) || IsMonochrome(id) || IsChroma(id) || IsReversebow(id) || IsVibrance(id) ||
        IsDarkbow(id) || IsAbberration(id);

    public static bool IsContrasting(int id) => id is 35 or 34 or 38 or 39 or 40;

    public static Color GetColor(int id, bool shadow) => id switch
    {
        41 => shadow ? ReversebowShadow : Reversebow,
        42 => shadow ? VibranceShadow : Vibrance,
        43 => shadow ? DarkbowShadow : Darkbow,
        44 => shadow ? AbberationShadow : Abberation,
        45 => shadow ? ChromaShadow : Chroma,
        46 => shadow ? MantleShadow : Mantle,
        47 => shadow ? FireShadow : Fire,
        48 => shadow ? GalaxyShadow : Galaxy,
        49 => shadow ? MonochromeShadow : Monochrome,
        50 => shadow ? RainbowShadow : Rainbow,
        _ => shadow ? Palette.ShadowColors[id] : Palette.PlayerColors[id]
    };

    public static readonly Dictionary<int, string> LightDarkColors = new()
    {
        { 0, "Darker" }, //Red
        { 1, "Darker" }, //Blue
        { 2, "Darker" }, //Green
        { 3, "Lighter" }, //Pink
        { 4, "Lighter" }, //Orange
        { 5, "Lighter" }, //Yellow
        { 6, "Darker" }, //Black
        { 7, "Lighter" }, //White
        { 8, "Darker" }, //Purple
        { 9, "Darker" }, //Brown
        { 10, "Lighter" }, //Cyan
        { 11, "Lighter" }, //Lime
        { 12, "Darker" }, //Maroon
        { 13, "Lighter" }, //Rose
        { 14, "Lighter" }, //Banana
        { 15, "Darker" }, //Grey
        { 16, "Darker" }, //Tan
        { 17, "Lighter" }, //Coral
        //New colors
        { 18, "Darker" }, //Watermelon
        { 19, "Darker" }, //Chocolate
        { 20, "Lighter" }, //Sky Blue
        { 21, "Lighter" }, //Beige
        { 22, "Darker" }, //Magenta
        { 23, "Lighter" }, //Turquoise
        { 24, "Lighter" }, //Lilac
        { 25, "Darker" }, //Olive
        { 26, "Lighter" }, //Azure
        { 27, "Darker" }, //Plum
        { 28, "Darker" }, //Jungle
        { 29, "Lighter" }, //Mint
        { 30, "Lighter" }, //Chartreuse
        { 31, "Darker" }, //Macau
        { 32, "Darker" }, //Tawny
        { 33, "Lighter" }, //Gold
        { 34, "Lighter" }, //Panda
        { 35, "Darker" }, //Contrast
        { 36, "Lighter" }, //Starlight
        { 37, "Darker" }, //Vantablack
        { 38, "Lighter" }, //Ice
        { 39, "Darker" }, //Nougat
        { 40, "Lighter" }, //Lava
        //Everchanging colors
        { 41, "Darker" }, //Reversebow
        { 42, "Lighter" }, //Vibrance
        { 43, "Darker" }, //Darkbow
        { 44, "Darker" }, //Aberration
        { 45, "Lighter" }, //Chroma
        { 46, "Darker" }, //Mantle
        { 47, "Lighter" }, //Fire
        { 48, "Lighter" }, //Galaxy
        { 49, "Lighter" }, //Monochrome
        { 50, "Lighter" } //Rainbow
    };

    public static void LoadColors()
    {
        Palette.ColorNames = new[]
        {
            StringNames.ColorRed,
            StringNames.ColorBlue,
            StringNames.ColorGreen,
            StringNames.ColorPink,
            StringNames.ColorOrange,
            StringNames.ColorYellow,
            StringNames.ColorBlack,
            StringNames.ColorWhite,
            StringNames.ColorPurple,
            StringNames.ColorBrown,
            StringNames.ColorCyan,
            StringNames.ColorLime,
            StringNames.ColorMaroon,
            StringNames.ColorRose,
            StringNames.ColorBanana,
            StringNames.ColorGray,
            StringNames.ColorTan,
            StringNames.ColorCoral,
            //New colors
            (StringNames)999967, //Watermelon
            (StringNames)999968, //Chocolate
            (StringNames)999969, //Sky Blue
            (StringNames)999970, //Beige
            (StringNames)999971, //Magenta
            (StringNames)999972, //Turquoise
            (StringNames)999973, //Lilac
            (StringNames)999974, //Olive
            (StringNames)999975, //Azure
            (StringNames)999976, //Plum
            (StringNames)999977, //Jungle
            (StringNames)999978, //Mint
            (StringNames)999979, //Chartreuse
            (StringNames)999980, //Macau
            (StringNames)999981, //Tawny
            (StringNames)999982, //Gold
            (StringNames)999983, //Panda
            (StringNames)999984, //Contrast
            (StringNames)999985, //Starlight
            (StringNames)999986, //Vantablack
            (StringNames)999987, //Ice
            (StringNames)999988, //Nougat
            (StringNames)999989, //Lava
            //Everchanging colors
            (StringNames)999990, //Reversebow
            (StringNames)999991, //Vibrance
            (StringNames)999992, //Darkbow
            (StringNames)999993, //Aberration
            (StringNames)999994, //Chroma
            (StringNames)999995, //Mantle
            (StringNames)999996, //Fire
            (StringNames)999997, //Galaxy
            (StringNames)999998, //Monochrome
            (StringNames)999999, //Rainbow
        };

        Palette.PlayerColors = new Color32[]
        {
            new(198, 17, 17, 255),
            new(19, 46, 210, 255),
            new(17, 128, 45, 255),
            new(238, 84, 187, 255),
            new(240, 125, 13, 255),
            new(246, 246, 87, 255),
            new(63, 71, 78, 255),
            new(215, 225, 241, 255),
            new(107, 47, 188, 255),
            new(113, 73, 30, 255),
            new(56, 255, 221, 255),
            new(80, 240, 57, 255),
            new(95, 29, 46, 255),
            new(236, 192, 211, 255),
            new(240, 231, 168, 255),
            new(117, 133, 147, 255),
            new(145, 136, 119, 255),
            new(215, 100, 100, 255),
            //New colors
            new(168, 50, 62, 255),
            new(60, 48, 44, 255),
            new(61, 129, 255, 255),
            new(240, 211, 165, 255),
            new(255, 0, 127, 255),
            new(61, 255, 181, 255),
            new(186, 161, 255, 255),
            new(97, 114, 24, 255),
            new(1, 166, 255, 255),
            new(79, 0, 127, 255),
            new(0, 47, 0, 255),
            new(151, 255, 151, 255),
            new(207, 255, 0, 255),
            new(0, 97, 93, 255),
            new(205, 63, 0, 255),
            new(255, 207, 0, 255),
            new(255, 255, 255, 255),
            new(255, 0, 0, 255),
            new(255, 255, 255, 255),
            new(15, 15, 15, 255),
            new(251, 251, 255, 255),
            new(160, 101, 56, 255),
            new(255, 255, 16, 255),
            //Everchanging colors
            new(0, 0, 0, 255),
            new(0, 0, 0, 255),
            new(0, 0, 0, 255),
            new(0, 0, 0, 255),
            new(0, 0, 0, 255),
            new(0, 0, 0, 255),
            new(0, 0, 0, 255),
            new(0, 0, 0, 255),
            new(0, 0, 0, 255),
            new(0, 0, 0, 255)
        };

        Palette.ShadowColors = new Color32[]
        {
            new(122, 8, 56, 255),
            new(9, 21, 142, 255),
            new(10, 77, 46, 255),
            new(172, 43, 174, 255),
            new(180, 62, 21, 255),
            new(195, 136, 34, 255),
            new(30, 31, 38, 255),
            new(132, 149, 192, 255),
            new(59, 23, 124, 255),
            new(94, 38, 21, 255),
            new(36, 169, 191, 255),
            new(21, 168, 66, 255),
            new(65, 15, 26, 255),
            new(222, 146, 179, 255),
            new(210, 188, 137, 255),
            new(70, 86, 100, 255),
            new(81, 65, 62, 255),
            new(180, 67, 98, 255),
            //New colors
            new(101, 30, 37, 255),
            new(30, 24, 22, 255),
            new(31, 65, 128, 255),
            new(120, 106, 83, 255),
            new(191, 0, 95, 255),
            new(31, 128, 91, 255),
            new(93, 81, 128, 255),
            new(66, 91, 15, 255),
            new(17, 104, 151, 255),
            new(55, 0, 95, 255),
            new(0, 23, 0, 255),
            new(109, 191, 109, 255),
            new(143, 191, 61, 255),
            new(0, 65, 61, 255),
            new(141, 31, 0, 255),
            new(191, 143, 0, 255),
            new(0, 0, 0, 255),
            new(0, 0, 255, 255),
            new(255, 255, 255, 255),
            new(0, 0, 0, 255),
            new(112, 250, 241, 255),
            new(115, 15, 78, 255),
            new(255, 0, 0, 255),
            //Everchanging colors
            new(0, 0, 0, 255),
            new(0, 0, 0, 255),
            new(0, 0, 0, 255),
            new(0, 0, 0, 255),
            new(0, 0, 0, 255),
            new(0, 0, 0, 255),
            new(0, 0, 0, 255),
            new(0, 0, 0, 255),
            new(0, 0, 0, 255),
            new(0, 0, 0, 255)
        };
    }

    [HarmonyPatch(typeof(TranslationController), nameof(TranslationController.GetString), typeof(StringNames), typeof(Il2CppReferenceArray<Il2CppSystem.Object>))]
    public static class PatchColours
    {
        public static bool Prefix(ref string __result, ref StringNames id)
        {
            var newResult = (int)id switch
            {
                999967 => "Watermelon",
                999968 => "Chocolate",
                999969 => "SkyBlue",
                999970 => "Beige",
                999971 => "Magenta",
                999972 => "Turquoise",
                999973 => "Lilac",
                999974 => "Olive",
                999975 => "Azure",
                999976 => "Plum",
                999977 => "Jungle",
                999978 => "Mint",
                999979 => "Chartreuse",
                999980 => "Macau",
                999981 => "Tawny",
                999982 => "Gold",
                999983 => "Panda",
                999984 => "Contrast",
                999985 => "Starlight",
                999986 => "Vantablack",
                999987 => "Ice",
                999988 => "Nougat",
                999989 => "Lava",
                999990 => "Reversebow",
                999991 => "Vibrance",
                999992 => "Darkbow",
                999993 => "Aberration",
                999994 => "Chroma",
                999995 => "Mantle",
                999996 => "Fire",
                999997 => "Galaxy",
                999998 => "Monochrome",
                999999 => "Rainbow",
                _ => null
            };

            if (newResult != null)
            {
                __result = Translate($"Colors.{newResult}");
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(PlayerTab), nameof(PlayerTab.OnEnable))]
    public static class PlayerTabOnEnablePatch
    {
        public static void Postfix(PlayerTab __instance)
        {
            for (var i = 0; i < __instance.ColorChips.Count; i++)
            {
                var colorChip = __instance.ColorChips[i];
                colorChip.transform.localScale *= 0.6f;
                var x = __instance.XRange.min + (i % 10 * 0.35f);
                var y = __instance.YStart - (i / 10 * 0.35f);
                colorChip.transform.localPosition = new(x, y, 2f);
            }
        }
    }

    [HarmonyPatch(typeof(PlayerTab), nameof(PlayerTab.Update))]
    public static class PlayerTabUpdatePatch
    {
        public static void Postfix(PlayerTab __instance)
        {
            for (var i = 0; i < __instance.ColorChips.Count; i++)
                __instance.ColorChips[i].Inner.SpriteColor = GetColor(i, false);
        }
    }

    [HarmonyPatch(typeof(PlayerMaterial), nameof(PlayerMaterial.SetColors), typeof(int), typeof(Renderer))]
    public static class SetPlayerMaterialPatch
    {
        public static bool Prefix(ref int colorId, ref Renderer rend)
        {
            var r = rend.gameObject.GetComponent<ColorBehaviour>() ?? rend.gameObject.AddComponent<ColorBehaviour>();
            r.AddRend(rend, colorId);
            return !IsChanging(colorId);
        }
    }

    [HarmonyPatch(typeof(PlayerMaterial), nameof(PlayerMaterial.SetColors), typeof(UColor), typeof(Renderer))]
    public static class SetPlayerMaterialPatch2
    {
        public static bool Prefix(ref Renderer rend)
        {
            var r = rend.gameObject.GetComponent<ColorBehaviour>() ?? rend.gameObject.AddComponent<ColorBehaviour>();
            r.AddRend(rend, 0);
            return true;
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdCheckColor))]
    public static class CmdCheckColorPatch
    {
        public static bool Prefix(PlayerControl __instance, ref byte bodyColor)
        {
            CallRpc(CustomRPC.Misc, MiscRPC.SetColor, __instance, bodyColor);
            __instance.SetColor(bodyColor);
            return false;
        }
    }

    [HarmonyPatch(typeof(PlayerTab), nameof(PlayerTab.UpdateAvailableColors))]
    public static class UpdateAvailableColorsPatch
    {
        public static bool Prefix(PlayerTab __instance)
        {
            __instance.AvailableColors.Clear();

            for (var i = 0; i < Palette.PlayerColors.Count; i++)
            {
                if (!CustomPlayer.Local || CustomPlayer.Local.CurrentOutfit.ColorId != i)
                    __instance.AvailableColors.Add(i);
            }

            return false;
        }
    }
}