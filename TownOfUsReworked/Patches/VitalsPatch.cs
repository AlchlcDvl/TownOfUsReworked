namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(VitalsMinigame))]
public static class VitalsPatch
{
    [HarmonyPatch(nameof(VitalsMinigame.Update)), HarmonyPostfix]
    public static void UpdatePostfix(VitalsMinigame __instance)
    {
        var role = CustomPlayer.Local.GetRole();
        var isOp = role is Operative || DeadSeeEverything();

        if (!isOp)
            isOp = role is Retributionist ret && ret.IsOp;

        if (!isOp)
            return;

        for (var i = 0; i < __instance.vitals.Count; i++)
        {
            var panel = __instance.vitals[i];
            var info = GameData.Instance.AllPlayers[i];

            if (!panel.IsDead || !KilledPlayers.TryFinding(x => x.PlayerId == info.PlayerId, out var deadBody))
                continue;

            var tmp = panel.Cardio.GetComponent<TextMeshPro>();
            tmp.color = UColor.red;
            tmp.text = $"{Mathf.RoundToInt(deadBody.KillAge)}s";
            var transform = tmp.transform;
            transform.localPosition = new(-0.85f, -0.4f, 0);
            transform.rotation = Quaternion.Euler(0, 0, 0);
            transform.localScale = Vector3.one / 20;
        }
    }

    [HarmonyPatch(nameof(VitalsMinigame.Begin)), HarmonyPostfix]
    public static void BeginPostfix(VitalsMinigame __instance) => __instance.AddComponent<VitalsPagingBehaviour>().Menu = __instance;
}