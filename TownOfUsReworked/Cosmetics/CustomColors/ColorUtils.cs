using UnityEngine;
using HarmonyLib;

namespace TownOfUsReworked.Cosmetics.CustomColors
{
    [HarmonyPatch]
    public static class ColorUtils
    {
        private static readonly int BackColor = Shader.PropertyToID("_BackColor");
        private static readonly int BodyColor = Shader.PropertyToID("_BodyColor");
        private static readonly int VisorColor = Shader.PropertyToID("_VisorColor");

        public static Color Rainbow => new HSBColor(PP(0f, 1f, 0.3f), 1f, 1f).ToColor();
        public static Color RainbowShadow => Shadow(Rainbow);

        public static Color Galaxy => new HSBColor(PP(0.5f, 0.87f, 0.4f), 1f, 1f).ToColor();
        public static Color GalaxyShadow => Shadow(Galaxy);

        public static Color Fire => new HSBColor(PP(0f, 0.17f, 0.4f), 1f, 1f).ToColor();
        public static Color FireShadow => Shadow(Fire);

        public static Color Monochrome => new HSBColor(1f, 0f, PP(0f, 1f, 0.8f)).ToColor();
        public static Color MonochromeShadow => Shadow(Monochrome);

        public static Color Mantle => new HSBColor(PP(0f, 1f, 0.3f), PP(0f, 1f, 0.3f), PP(0f, 1f, 0.3f)).ToColor();
        public static Color MantleShadow => Shadow(Mantle);

        public static Color Chroma => new HSBColor(PP(0f, 1f, 0.4f), PPR(0f, 1f, 0.6f), PP(0f, 1f, 0.9f)).ToColor();
        public static Color ChromaShadow => Shadow(Chroma);

        public static float PP(float min, float max, float mul) => min + Mathf.PingPong(Time.time * mul, max - min);

        public static float PPR(float min, float max, float mul) => max - Mathf.PingPong(Time.time * mul, max - min);

        public static Color Shadow(Color color) => new(color.r - 0.3f, color.g - 0.3f, color.b - 0.3f);

        public static void SetRainbow(Renderer rend)
        {
            rend.material.SetColor(BackColor, RainbowShadow);
            rend.material.SetColor(BodyColor, Rainbow);
            rend.material.SetColor(VisorColor, Palette.VisorColor);
        }

        public static void SetMonochrome(Renderer rend)
        {
            rend.material.SetColor(BackColor, MonochromeShadow);
            rend.material.SetColor(BodyColor, Monochrome);
            rend.material.SetColor(VisorColor, Palette.VisorColor);
        }

        public static void SetFire(Renderer rend)
        {
            rend.material.SetColor(BackColor, FireShadow);
            rend.material.SetColor(BodyColor, Fire);
            rend.material.SetColor(VisorColor, Palette.VisorColor);
        }

        public static void SetGalaxy(Renderer rend)
        {
            rend.material.SetColor(BackColor, GalaxyShadow);
            rend.material.SetColor(BodyColor, Galaxy);
            rend.material.SetColor(VisorColor, Palette.VisorColor);
        }

        public static void SetMantle(Renderer rend)
        {
            rend.material.SetColor(BackColor, MantleShadow);
            rend.material.SetColor(BodyColor, Mantle);
            rend.material.SetColor(VisorColor, Palette.VisorColor);
        }

        public static void SetChroma(Renderer rend)
        {
            rend.material.SetColor(BackColor, ChromaShadow);
            rend.material.SetColor(BodyColor, Chroma);
            rend.material.SetColor(VisorColor, Palette.VisorColor);
        }

        private static bool OutOfBounds(int id) => id < 0 || id >= Palette.ColorNames.Count;

        public static bool IsRainbow(int id) => !OutOfBounds(id) && (int)Palette.ColorNames[id] == 999999;

        public static bool IsMonochrome(int id) => !OutOfBounds(id) && (int)Palette.ColorNames[id] == 999998;

        public static bool IsGalaxy(int id) => !OutOfBounds(id) && (int)Palette.ColorNames[id] == 999997;

        public static bool IsFire(int id) => !OutOfBounds(id) && (int)Palette.ColorNames[id] == 999996;

        public static bool IsMantle(int id) => !OutOfBounds(id) && (int)Palette.ColorNames[id] == 999995;

        public static bool IsChroma(int id) => !OutOfBounds(id) && (int)Palette.ColorNames[id] == 999994;

        public static bool IsChanging(int id) => IsRainbow(id) || IsFire(id) || IsGalaxy(id) || IsMantle(id) || IsMonochrome(id) || IsChroma(id);
    }
}