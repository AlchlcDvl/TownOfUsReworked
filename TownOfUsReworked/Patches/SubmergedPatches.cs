namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.ShowRole))]
public static class SubmergedStartPatch
{
    public static void Postfix()
    {
        if (CustomPlayer.Local == null || CustomPlayer.Local.Data == null || !IsSubmerged())
            return;

        Coroutines.Start(WaitStart(() => ButtonUtils.Reset(CooldownType.Start)));
    }
}

[HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
public static class SubmergedHudPatch
{
    public static void Postfix(HudManager __instance)
    {
        if (IsSubmerged() && CustomPlayer.Local.IsPostmortal())
            __instance.MapButton.transform.parent.Find(__instance.MapButton.name + "(Clone)").gameObject.SetActive(CustomPlayer.Local.Caught());
    }
}

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.HandleAnimation))]
public static class SubmergedPhysicsPatch
{
    public static void Postfix(PlayerPhysics __instance) => Ghostrolefix(__instance);
}

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class SubmergedLateUpdatePhysicsPatch
{
    public static void Postfix(PlayerPhysics __instance) => Ghostrolefix(__instance);
}

[HarmonyPatch(typeof(UObject), nameof(UObject.Destroy), typeof(UObject))]
public static class SubmergedExile
{
    public static void Prefix(UObject obj)
    {
        if (!IsSubmerged() || obj == null || obj.name == null)
            return;

        if (obj.name.Contains("ExileCutscene"))
            SetPostmortals.ExileControllerPostfix(Ejection);
        else if (obj.name.Contains("SpawnInMinigame") && CustomPlayer.Local.Is(LayerEnum.Astral) && !CustomPlayer.LocalCustom.IsDead)
            Modifier.GetModifier<Astral>(CustomPlayer.Local).SetPosition();
    }
}