namespace TownOfUsReworked.Localisation;

[HarmonyPatch(typeof(TranslationController), nameof(TranslationController.GetString), typeof(StringNames), typeof(Il2CppReferenceArray<Il2CppSystem.Object>))]
public static class PatchColours
{
    public static bool Prefix(ref string __result, ref StringNames id)
    {
        var id2 = id;
        var color = CustomColorManager.AllColors.Find(x => x.StringID == (int)id2 && !x.Default);

        if (color != null)
        {
            var result = TranslationManager.Translate($"Colors.{color.Name}");
            __result = result == $"Colors.{color.Name}" ? color.Name : result;
            return false;
        }

        return true;
    }
}