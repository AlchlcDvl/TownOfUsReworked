namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(IntroCutscene._ShowRole_d__39), nameof(IntroCutscene._ShowRole_d__39.MoveNext))]
public static class SubmergedStartPatch
{
    public static void Postfix()
    {
        if (PlayerControl.LocalPlayer.Data == null || PlayerControl.LocalPlayer == null)
            return;

        if (IsSubmerged)
            Coroutines.Start(WaitStart(() => ButtonUtils.ResetCustomTimers(true)));
    }
}

[HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
public static class SubmergedHudPatch
{
    public static void Postfix(HudManager __instance)
    {
        if (IsSubmerged && CustomPlayer.Local.IsPostmortal())
            __instance.MapButton.transform.parent.Find(__instance.MapButton.name + "(Clone)").gameObject.SetActive(CustomPlayer.Local.Caught());
    }
}

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.HandleAnimation))]
[HarmonyPriority(Priority.Low)] //Make sure it occurs after other patches
public static class SubmergedPhysicsPatch
{
    public static void Postfix(PlayerPhysics __instance) => Ghostrolefix(__instance);
}

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
[HarmonyPriority(Priority.Low)] //Make sure it occurs after other patches
public static class SubmergedLateUpdatePhysicsPatch
{
    public static void Postfix(PlayerPhysics __instance) => Ghostrolefix(__instance);
}