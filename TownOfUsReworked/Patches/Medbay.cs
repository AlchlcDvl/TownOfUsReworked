namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(MedScanMinigame))]
public static class MedScanMinigamePatch
{
    [HarmonyPatch(nameof(MedScanMinigame.Begin))]
    public static void Postfix(MedScanMinigame __instance)
    {
        var newHeightFeet = 0f;
        var newHeightInch = 6f * CustomPlayer.Local.GetModifiedSize();
        var newWeight = 92f * CustomPlayer.Local.GetModifiedSize();

        if (newHeightFeet < 0f)
            newHeightFeet = 0f;

        while (newHeightInch >= 12)
        {
            newHeightFeet++;
            newHeightInch -= 12;
        }

        var weightString = $"{(int)newWeight}lb";
        var heightString = $"{(int)newHeightFeet}' {(int)newHeightInch}\"";
        __instance.completeString = __instance.completeString.Replace("3' 6\"", heightString).Replace("92lb", weightString);
    }

    [HarmonyPatch(nameof(MedScanMinigame.FixedUpdate))]
    public static void Prefix(MedScanMinigame __instance)
    {
        if (GameModifiers.ParallelMedScans)
        {
            // Allows multiple medbay scans at once
            __instance.medscan.CurrentUser = CustomPlayer.Local.PlayerId;
            __instance.medscan.UsersList.Clear();
        }
    }
}