using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace TownOfUsReworked.Cosmetics
{
    [HarmonyPatch(typeof(TranslationController), nameof(TranslationController.GetString), new[] { typeof(StringNames), typeof(Il2CppReferenceArray<Il2CppSystem.Object>) })]
    public class PatchColours
    {
        public static bool Prefix(ref string __result, [HarmonyArgument(0)] StringNames name)
        {
            var newResult = (int)name switch
            {
                999983 => "Watermelon",
                999984 => "Chocolate",
                999985 => "Sky Blue",
                999986 => "Beige",
                999987 => "Magenta",
                999988 => "Lilac",
                999989 => "Turquoise",
                999990 => "Olive",
                999991 => "Azure",
                999992 => "Plum",
                999993 => "Jungle",
                999994 => "Mint",
                999995 => "Chartreuse",
                999996 => "Macau",
                999997 => "Tawny",
                999998 => "Gold",
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
