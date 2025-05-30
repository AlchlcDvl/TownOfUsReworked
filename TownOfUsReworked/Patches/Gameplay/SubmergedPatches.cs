namespace TownOfUsReworked.Patches.Gameplay;

[HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.ShowRole))]
public static class SubmergedStartPatch
{
    public static void Postfix()
    {
        if (!LocalPlayer || !LocalPlayer.Data || !IsSubmerged())
            return;

        Coroutines.Start(WaitAction(() => ButtonUtils.Reset(CooldownType.Start)));
    }
}

[HarmonyPatch(typeof(PlayerPhysics)), HarmonyPriority(Priority.Low)]
public static class SubmergedPhysicsPatches
{
    [HarmonyPatch(nameof(PlayerPhysics.HandleAnimation))]
    [HarmonyPatch(nameof(PlayerPhysics.LateUpdate))]
    public static void Postfix(PlayerPhysics __instance) => GhostRoleFix(__instance);
}