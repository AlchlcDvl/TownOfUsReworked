using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace TownOfUsReworked.Lobby.Extras.RainbowMod
{
    [HarmonyPatch(typeof(TranslationController), nameof(TranslationController.GetString), new[] { typeof(StringNames),
        typeof(Il2CppReferenceArray<Il2CppSystem.Object>) })]
    public class PatchColours
    {
        public static bool Prefix(ref string __result, [HarmonyArgument(0)] StringNames name)
        {
            var newResult = (int)name switch
            {
                999903 => "Watermelon",
                999904 => "Chocolate",
                999905 => "Sky Blue",
                999906 => "Beige",
                999907 => "Hot Pink",
                999908 => "Turquoise",
                999909 => "Lilac",
                999910 => "Olive",
                999911 => "Azure",
                999912 => "Tomato",
                999913 => "Backrooms",
                999914 => "Gold",
                999915 => "Space",
                999916 => "Ice",
                999917 => "Mint",
                999918 => "Behind the Slaughter",
                999919 => "Forest Green",
                999920 => "Donation",
                999921 => "Cherry",
                999922 => "Toy",
                999923 => "Pizzaria",
                999924 => "Starlight",
                999925 => "Softball",
                999926 => "Dark Jester",
                999927 => "FRESH",
                999928 => "Goner.",
                999929 => "Psychic Friend",
                999930 => "Frost",
                999931 => "Abyss Green",
                999932 => "Midnight",
                999933 => "<3",
                999934 => "Heat From Fire",
                999935 => "Fire From Heat",
                999936 => "Determination",
                999937 => "Patience",
                999938 => "Bravery",
                999939 => "Integrity",
                999940 => "Perserverance",
                999941 => "Kindness",
                999942 => "Justice",
                999943 => "Purple Plumber",
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
