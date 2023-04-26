using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace TownOfUsReworked.Cosmetics
{
    [HarmonyPatch(typeof(TranslationController), nameof(TranslationController.GetString), new[] { typeof(StringNames), typeof(Il2CppReferenceArray<Il2CppSystem.Object>) })]
    public static class PatchColours
    {
        public static bool Prefix(ref string __result, [HarmonyArgument(0)] StringNames name)
        {
            var newResult = (int)name switch
            {
                999976 => "Watermelon",
                999977 => "Chocolate",
                999978 => "Sky Blue",
                999979 => "Beige",
                999980 => "Magenta",
                999981 => "Turquoise",
                999982 => "Lilac",
                999983 => "Olive",
                999984 => "Azure",
                999985 => "Plum",
                999986 => "Jungle",
                999987 => "Mint",
                999988 => "Chartreuse",
                999989 => "Macau",
                999990 => "Tawny",
                999991 => "Gold",
                999992 => "Panda",
                999993 => "Contrast",
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
                __result = newResult;
                return false;
            }

            return true;
        }
    }
}