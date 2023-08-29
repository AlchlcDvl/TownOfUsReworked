namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(Vent), nameof(Vent.CanUse))]
public static class VentPatches
{
    public static void Postfix(Vent __instance, [HarmonyArgument(0)] GameData.PlayerInfo playerInfo, [HarmonyArgument(1)] ref bool canUse, [HarmonyArgument(2)] ref bool couldUse, ref float
        __result)
    {
        var num = float.MaxValue;
        var playerControl = playerInfo.Object;

        if (IsNormal)
            couldUse = playerControl.CanVent();
        else if (IsHnS)
            couldUse = !playerInfo.IsImpostor();
        else
            couldUse = canUse;

        var ventitaltionSystem = ShipStatus.Instance.Systems[SystemTypes.Ventilation].Cast<VentilationSystem>();

        if (ventitaltionSystem != null && ventitaltionSystem.IsVentCurrentlyBeingCleaned(__instance.Id))
            couldUse = false;

        canUse = couldUse;

        if (canUse)
        {
            var center = playerControl.Collider.bounds.center;
            var position = __instance.transform.position;
            num = Vector2.Distance((Vector2)center, (Vector2)position);

            if (__instance.Id == 14 && IsSubmerged)
                canUse &= num <= __instance.UsableDistance;
            else
            {
                canUse = ((canUse ? 1 : 0) & (num > __instance.UsableDistance ? 0 : (!PhysicsHelpers.AnythingBetween(playerControl.Collider, (Vector2)center, (Vector2)position,
                    Constants.ShipOnlyMask, false) ? 1 : 0))) != 0;
            }
        }

        __result = num;
    }
}

[HarmonyPatch(typeof(Vent), nameof(Vent.SetButtons))]
public static class EnterVentPatch
{
    public static bool Prefix()
    {
        var player = CustomPlayer.Local;

        if (player.Is(LayerEnum.Jester) && CustomGameOptions.JesterVent)
            return CustomGameOptions.JestVentSwitch;
        else if (player.Is(LayerEnum.Executioner) && CustomGameOptions.ExeVent)
            return CustomGameOptions.ExeVentSwitch;
        else if (player.Is(LayerEnum.Survivor) && CustomGameOptions.SurvVent)
            return CustomGameOptions.SurvVentSwitch;
        else if (player.Is(LayerEnum.Amnesiac) && CustomGameOptions.AmneVent)
            return CustomGameOptions.AmneVentSwitch;
        else if (player.Is(LayerEnum.GuardianAngel) && CustomGameOptions.GAVent)
            return CustomGameOptions.GAVentSwitch;
        else if (player.Is(LayerEnum.Guesser) && CustomGameOptions.GuessVent)
            return CustomGameOptions.GuessVentSwitch;
        else if (player.Is(LayerEnum.Troll) && CustomGameOptions.TrollVent)
            return CustomGameOptions.TrollVentSwitch;
        else if (player.Is(LayerEnum.Actor) && CustomGameOptions.ActorVent)
            return CustomGameOptions.ActVentSwitch;
        else if (player.IsPostmortal())
            return false;
        else
            return !player.IsMoving();
    }
}

//Vent and kill shit
//Yes thank you Discussions - AD
[HarmonyPatch(typeof(Vent), nameof(Vent.SetOutline))]
public static class SetVentOutlinePatch
{
    public static void Postfix(Vent __instance, [HarmonyArgument(1)] ref bool mainTarget)
    {
        var active = CustomPlayer.Local && !Meeting && CustomPlayer.Local.CanVent();

        if (!Role.LocalRole || !active)
            return;

        __instance.myRend.material.SetColor("_OutlineColor", Role.LocalRole.Color);
        __instance.myRend.material.SetColor("_AddColor", mainTarget ? Role.LocalRole.Color : UColor.clear);
    }
}