namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(MedScanMinigame), nameof(MedScanMinigame.Begin))]
public static class MedScanMinigamePatch
{
    private const float oldHeightFeet = 3f;
    private const float oldHeightInch = 6f;
    private const float oldWeight = 92f;

    public static void Postfix(MedScanMinigame __instance)
    {
        var newHeightFeet = oldHeightFeet * CustomPlayer.Local.GetModifiedSize();
        var newHeightInch = oldHeightInch * CustomPlayer.Local.GetModifiedSize();
        var newWeight = oldWeight * CustomPlayer.Local.GetModifiedSize();

        while (newHeightFeet.IsInRange(0, 1))
        {
            newHeightInch += 12 * newHeightFeet;
            newHeightFeet--;
        }

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
}

[HarmonyPatch(typeof(MedScanMinigame), nameof(MedScanMinigame.FixedUpdate))]
public static class MedScanMinigameFixedUpdatePatch
{
    public static void Prefix(MedScanMinigame __instance)
    {
        if (CustomGameOptions.ParallelMedScans)
        {
            //Allows multiple medbay scans at once
            __instance.medscan.CurrentUser = CustomPlayer.Local.PlayerId;
            __instance.medscan.UsersList.Clear();
        }
    }
}