namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(VitalsMinigame), nameof(VitalsMinigame.Update))]
public static class VitalsPatch
{
    public static void Postfix(VitalsMinigame __instance)
    {
        var localPlayer = CustomPlayer.Local;
        var isOp = localPlayer.Is(LayerEnum.Operative) || DeadSeeEverything();

        if (!isOp)
            isOp = localPlayer.Is(LayerEnum.Retributionist) && ((Retributionist)Role.LocalRole).IsOp;

        if (!isOp)
            return;

        for (var i = 0; i < __instance.vitals.Count; i++)
        {
            var panel = __instance.vitals[i];
            var info = GameData.Instance.AllPlayers[i];

            if (!panel.IsDead || !KilledPlayers.TryFinding(x => x.PlayerId == info.PlayerId, out var deadBody))
                continue;

            var num = (float)(DateTime.UtcNow - deadBody.KillTime).TotalMilliseconds;
            var tmp = panel.Cardio.GetComponent<TextMeshPro>();
            tmp.color = UColor.red;
            tmp.SetText($"{Math.Ceiling(num / 1000)}s");
            var transform = tmp.transform;
            transform.localPosition = new(-0.85f, -0.4f, 0);
            transform.rotation = Quaternion.Euler(0, 0, 0);
            transform.localScale = Vector3.one / 20;
        }
    }
}

[HarmonyPatch(typeof(VitalsMinigame), nameof(VitalsMinigame.Begin))]
public static class VitalsMinigameBeginPatch
{
    public static void Postfix(VitalsMinigame __instance) => __instance.AddComponent<VitalsPagingBehaviour>().Menu = __instance;
}