﻿namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.ShowRole))]
public static class SubmergedStartPatch
{
    public static void Postfix()
    {
        if (!CustomPlayer.Local || !CustomPlayer.Local.Data || !IsSubmerged())
            return;

        Coroutines.Start(WaitStart(() => ButtonUtils.Reset(CooldownType.Start)));
    }
}

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.HandleAnimation))]
[HarmonyPriority(Priority.Low)]
public static class SubmergedPhysicsPatches1
{
    public static void Postfix(PlayerPhysics __instance) => GhostRoleFix(__instance);
}

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
[HarmonyPriority(Priority.Low)]
public static class SubmergedPhysicsPatches2
{
    public static void Postfix(PlayerPhysics __instance) => GhostRoleFix(__instance);
}

[HarmonyPatch(typeof(UObject), nameof(UObject.Destroy), typeof(UObject))]
public static class SubmergedExile
{
    public static void Prefix(UObject obj)
    {
        if (!IsSubmerged() || !obj || obj.name == null)
            return;

        if (obj.name.Contains("ExileCutscene"))
            SetPostmortals.ExileControllerPostfix(Ejection());
        else if (obj.name.Contains("SpawnInMinigame") && CustomPlayer.Local.TryGetLayer<Astral>(out var ast) && !CustomPlayer.LocalCustom.Dead)
        {
            ast.SetPosition();
            SetFullScreenHUD();
        }
    }
}