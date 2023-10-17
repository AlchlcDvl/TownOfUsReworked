namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(Vent), nameof(Vent.CanUse))]
public static class VentPatches
{
    public static void Postfix(Vent __instance, ref GameData.PlayerInfo pc, ref bool canUse, ref bool couldUse, ref float __result)
    {
        var num = float.MaxValue;
        var playerControl = pc.Object;

        if (IsNormal)
            couldUse = playerControl.CanVent();
        else if (IsHnS)
            couldUse = !pc.IsImpostor();
        else
            couldUse = canUse;

        var ventitaltionSystem = ShipStatus.Instance.Systems[SystemTypes.Ventilation].Cast<VentilationSystem>();

        if (ventitaltionSystem != null && ventitaltionSystem.IsVentCurrentlyBeingCleaned(__instance.Id))
            couldUse = false;

        canUse = couldUse;
        var center = playerControl.Collider.bounds.center;
        var position = __instance.transform.position;

        if (IsSubmerged)
        {
            if (GetInTransition())
            {
                __result = float.MaxValue;
                return;
            }

            switch (__instance.Id)
            {
                case 9:  //Engine Room Exit Only Vent
                    if (CustomPlayer.Local.inVent)
                        break;

                    __result = float.MaxValue;
                    return;

                case 14: //Lower Central
                    __result = float.MaxValue;

                    if (canUse)
                    {
                        __result = Vector2.Distance(center, position);
                        canUse &= __result <= __instance.UsableDistance;
                    }

                    return;
            }
        }

        if (canUse)
        {
            num = Vector2.Distance(center, position);

            if (__instance.Id == 14 && IsSubmerged)
                canUse &= num <= __instance.UsableDistance;
            else
            {
                canUse = ((canUse ? 1 : 0) & (num > __instance.UsableDistance ? 0 : (!PhysicsHelpers.AnythingBetween(playerControl.Collider, center, position, Constants.ShipOnlyMask, false)
                    ? 1 : 0))) != 0;
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
    public static void Postfix(Vent __instance, ref bool mainTarget)
    {
        var active = CustomPlayer.Local && !Meeting && CustomPlayer.Local.CanVent();

        if (!Role.LocalRole || !active)
            return;

        __instance.myRend.material.SetColor("_OutlineColor", Role.LocalRole.Color);
        __instance.myRend.material.SetColor("_AddColor", mainTarget ? Role.LocalRole.Color : UColor.clear);
    }
}

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.RpcEnterVent))]
public static class TryToEnterVentPatch
{
    public static bool Prefix(PlayerPhysics __instance, ref int id)
    {
        var vent = VentById(id);

        if (vent.IsBombed())
        {
            RpcMurderPlayer(__instance.myPlayer);
            Role.BastionBomb(vent, CustomGameOptions.BombRemovedOnKill);
            CallRpc(CustomRPC.Misc, MiscRPC.BastionBomb, vent);
            return false;
        }

        return true;
    }
}

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.RpcExitVent))]
public static class TryToExitVentPatch
{
    public static bool Prefix(PlayerPhysics __instance, ref int id)
    {
        var vent = VentById(id);

        if (vent.IsBombed())
        {
            RpcMurderPlayer(__instance.myPlayer);
            Role.BastionBomb(vent, CustomGameOptions.BombRemovedOnKill);
            CallRpc(CustomRPC.Misc, MiscRPC.BastionBomb, vent);
            return false;
        }

        return true;
    }
}

[HarmonyPatch(typeof(Vent), nameof(Vent.Use))]
public static class UseVent
{
    public static bool Prefix(Vent __instance)
    {
        if (NoPlayers)
            return true;

        if (!CustomPlayer.Local.CanVent())
            return false;

        if (__instance.IsBombed())
        {
            RpcMurderPlayer(CustomPlayer.Local);
            Role.BastionBomb(__instance, CustomGameOptions.BombRemovedOnKill);
            CallRpc(CustomRPC.Misc, MiscRPC.BastionBomb, __instance);
            return false;
        }

        return LocalNotBlocked;
    }
}

[HarmonyPatch(typeof(Vent), nameof(Vent.TryMoveToVent))]
public static class MoveToVentPatch
{
    public static bool Prefix(ref Vent otherVent)
    {
        if (NoPlayers)
            return true;

        if (!CustomPlayer.Local.CanVent())
            return false;

        if (otherVent.IsBombed())
        {
            RpcMurderPlayer(CustomPlayer.Local);
            Role.BastionBomb(otherVent, CustomGameOptions.BombRemovedOnKill);
            CallRpc(CustomRPC.Misc, MiscRPC.BastionBomb, otherVent);
            return false;
        }

        return LocalNotBlocked;
    }
}