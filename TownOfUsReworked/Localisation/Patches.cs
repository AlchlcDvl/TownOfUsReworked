namespace TownOfUsReworked.Localisation;

[HarmonyPatch(typeof(TranslationController), nameof(TranslationController.GetString), typeof(StringNames), typeof(Il2CppReferenceArray<Il2CppSystem.Object>))]
public static class PatchColours
{
    public static bool Prefix(StringNames id, ref string __result)
    {
        var result = CustomColorManager.AllColors.TryFinding(x => x.StringID == (int)id && !x.Default, out var color) && color != null;

        if (result)
        {
            var translation = TranslationManager.Translate($"Colors.{color.Name}");
            __result = translation == $"Colors.{color.Name}" ? color.Name : translation;
        }

        return !result;
    }
}