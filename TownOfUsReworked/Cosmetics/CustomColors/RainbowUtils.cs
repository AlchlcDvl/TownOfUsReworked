using UnityEngine;

namespace TownOfUsReworked.Cosmetics
{
    public class RainbowUtils
    {
        private static readonly int BackColor = Shader.PropertyToID("_BackColor");
        private static readonly int BodyColor = Shader.PropertyToID("_BodyColor");
        private static readonly int VisorColor = Shader.PropertyToID("_VisorColor");

        public static Color Rainbow => new HSBColor(PP(0, 1, 0.3f), 1, 1).ToColor();
        public static Color RainbowShadow => Shadow(Rainbow);

        public static Color Galaxy => new HSBColor(PP(0.5f, 0.87f, 0.4f), 1, 1).ToColor();
        public static Color GalaxyShadow => Shadow(Galaxy);

        public static Color Fire => new HSBColor(PP(0f, 0.17f, 0.4f), 1, 1).ToColor();
        public static Color FireShadow => Shadow(Fire);

        public static float PP(float min, float max, float mul)
        {
            return min + Mathf.PingPong(Time.time * mul, max - min);
        }

        public static Color Shadow(Color color)
        {
            return new Color(color.r - 0.3f, color.g - 0.3f, color.b - 0.3f);
        }

        public static void SetRainbow(Renderer rend)
        {
            rend.material.SetColor(BackColor, RainbowShadow);
            rend.material.SetColor(BodyColor, Rainbow);
            rend.material.SetColor(VisorColor, Palette.VisorColor);
        }

        public static bool IsRainbow(int id)
        {
            if (id < 0 || id >= Palette.ColorNames.Count)
                return false;

            return (int)Palette.ColorNames[id] == 999999;
        }
    }
}