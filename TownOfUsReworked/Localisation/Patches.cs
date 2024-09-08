namespace TownOfUsReworked.Localisation;

[HarmonyPatch(typeof(TranslationController), nameof(TranslationController.GetString), typeof(StringNames), typeof(Il2CppReferenceArray<Il2CppSystem.Object>))]
public static class PatchColours
{
    public static bool Prefix(StringNames id, ref string __result)
    {
        var id2 = id;

        if (CustomColorManager.AllColors.TryFinding(x => x.StringID == (int)id2 && !x.Default, out var color) && color != null)
        {
            var result = TranslationManager.Translate($"Colors.{color.Name}");
            __result = result == $"Colors.{color.Name}" ? color.Name : result;
            return false;
        }

        return true;
    }
}