namespace TownOfUsReworked.Patches;

#region OpenDoorConsole
[HarmonyPatch(typeof(OpenDoorConsole))]
public static class OpenDoorConsolePatches
{
    [HarmonyPatch(nameof(OpenDoorConsole.CanUse))]
    public static void Prefix(NetworkedPlayerInfo pc, ref bool __state) => CanUsePatch.Prefix(pc.Object, ref __state);

    [HarmonyPatch(nameof(OpenDoorConsole.CanUse))]
    public static void Postfix(NetworkedPlayerInfo pc, ref bool __state) => CanUsePatch.Postfix(pc.Object, ref __state);

    [HarmonyPatch(nameof(OpenDoorConsole.Use))]
    public static bool Prefix(OpenDoorConsole __instance)
    {
        __instance.CanUse(CustomPlayer.LocalCustom.Data, out var canUse, out _);

        if (canUse)
        {
            Ship().RpcUpdateSystem(SystemTypes.Doors, (byte)(__instance.myDoor.Id | 64));
            CallRpc(CustomRPC.Misc, MiscRPC.DoorSyncToilet, __instance.myDoor.Id);
            __instance.myDoor.SetDoorway(true);
        }

        return false;
    }
}
#endregion

#region DoorConsole
[HarmonyPatch(typeof(DoorConsole), nameof(DoorConsole.CanUse))]
public static class DoorConsoleCanUse
{
    public static void Prefix(NetworkedPlayerInfo pc, ref bool __state) => CanUsePatch.Prefix(pc.Object, ref __state);

    public static void Postfix(NetworkedPlayerInfo pc, ref bool __state) => CanUsePatch.Postfix(pc.Object, ref __state);
}
#endregion

#region Ladder
[HarmonyPatch(typeof(Ladder), nameof(Ladder.CanUse))]
public static class LadderCanUse
{
    public static void Prefix(NetworkedPlayerInfo pc, ref bool __state) => CanUsePatch.Prefix(pc.Object, ref __state);

    public static void Postfix(NetworkedPlayerInfo pc, ref bool __state) => CanUsePatch.Postfix(pc.Object, ref __state);
}
#endregion

#region PlatformConsole
[HarmonyPatch(typeof(PlatformConsole), nameof(PlatformConsole.CanUse))]
public static class PlatformConsoleCanUse
{
    public static void Prefix(NetworkedPlayerInfo pc, ref bool __state) => CanUsePatch.Prefix(pc.Object, ref __state);

    public static void Postfix(NetworkedPlayerInfo pc, ref bool __state) => CanUsePatch.Postfix(pc.Object, ref __state);
}

[HarmonyPatch(typeof(MovingPlatformBehaviour), nameof(MovingPlatformBehaviour.Use), typeof(PlayerControl))]
public static class MovingPlatformBehaviourUse
{
    public static void Prefix(PlayerControl player, ref bool __state) => CanUsePatch.Prefix(player, ref __state);

    public static void Postfix(PlayerControl player, ref bool __state) => CanUsePatch.Postfix(player, ref __state);
}
#endregion

#region DeconControl
[HarmonyPatch(typeof(DeconControl), nameof(DeconControl.CanUse))]
public static class DeconControlUse
{
    public static void Prefix(NetworkedPlayerInfo pc, ref bool __state) => CanUsePatch.Prefix(pc.Object, ref __state);

    public static void Postfix(NetworkedPlayerInfo pc, ref bool __state) => CanUsePatch.Postfix(pc.Object, ref __state);
}
#endregion

#region Console
[HarmonyPatch(typeof(Console))]
public static class ConsoleCanUsePatch
{
    [HarmonyPatch(nameof(Console.CanUse))]
    public static bool Prefix(Console __instance, NetworkedPlayerInfo pc, ref float __result, ref bool canUse, ref bool couldUse, ref bool __state)
    {
        CanUsePatch.Prefix(pc.Object, ref __state);

        // If the console is not a sabotage repair console
        if (!pc.Object.CanDoTasks() && !__instance.AllowImpostor)
        {
            __result = float.MaxValue;
            canUse = couldUse = false;
            return false;
        }

        return true;
    }

    [HarmonyPatch(nameof(Console.CanUse))]
    public static void Postfix(NetworkedPlayerInfo pc, ref bool __state) => CanUsePatch.Postfix(pc.Object, ref __state);

    [HarmonyPatch(nameof(Console.SetOutline))]
    public static void Postfix(Console __instance, bool mainTarget)
    {
        if (!CustomPlayer.Local.TryGetLayer<Role>(out var role) || Meeting())
            return;

        __instance.Image.material.SetColor("_OutlineColor", role.Color);
        __instance.Image.material.SetColor("_AddColor", mainTarget ? role.Color : UColor.clear);
    }
}
#endregion

#region ZiplineConsole
[HarmonyPatch(typeof(ZiplineConsole), nameof(ZiplineConsole.CanUse))]
public static class ZiplineConsoleCanUse
{
    public static void Prefix(NetworkedPlayerInfo pc, ref bool __state) => CanUsePatch.Prefix(pc.Object, ref __state);

    public static void Postfix(NetworkedPlayerInfo pc, ref bool __state) => CanUsePatch.Postfix(pc.Object, ref __state);
}

[HarmonyPatch(typeof(ZiplineBehaviour), nameof(ZiplineBehaviour.Use), typeof(PlayerControl), typeof(bool))]
public class ZiplineBehaviourUse
{
    public static void Postfix(PlayerControl player, ref bool __state) => CanUsePatch.Postfix(player, ref __state);

    public static void Prefix(ZiplineBehaviour __instance, PlayerControl player, bool fromTop, ref bool __state)
    {
        CanUsePatch.Prefix(player, ref __state);

        try
        {
            UninteractiblePlayers.TryAdd(player.PlayerId, Time.time);
            UninteractiblePlayers2.TryAdd(player.PlayerId, fromTop ? __instance.upTravelTime : __instance.downTravelTime);
            CallRpc(CustomRPC.Action, ActionsRPC.SetUninteractable, player, UninteractiblePlayers2[player.PlayerId], true);
            var hand = __instance.playerIdHands[player.PlayerId];

            if (player.GetCustomOutfitType() is CustomPlayerOutfitType.Invis or CustomPlayerOutfitType.PlayerNameOnly)
                hand.handRenderer.color.SetAlpha(player.MyRend().color.a);
            else if (player.GetCustomOutfitType() == CustomPlayerOutfitType.Camouflage)
                PlayerMaterial.SetColors(UColor.grey, hand.handRenderer);
            else if (player.GetCustomOutfitType() == CustomPlayerOutfitType.Colorblind)
                hand.handRenderer.color = UColor.grey;
            else if (player.IsMimicking(out var mimicked))
                hand.SetPlayerColor(mimicked.GetCurrentOutfit(), PlayerMaterial.MaskType.None, mimicked.cosmetics.GetPhantomRoleAlpha());
            else
                hand.SetPlayerColor(player.GetCurrentOutfit(), PlayerMaterial.MaskType.None, player.cosmetics.GetPhantomRoleAlpha());
        }
        catch (Exception e)
        {
            Error(e);
        }

        if (CustomPlayer.Local.TryGetLayer<Astral>(out var ast))
            ast.LastPosition = CustomPlayer.LocalCustom.Position;
    }
}
#endregion

#region Other
[HarmonyPatch(typeof(DoorCardSwipeGame), nameof(DoorCardSwipeGame.Begin))]
public static class DoorSwipePatch
{
    public static void Prefix(DoorCardSwipeGame __instance) => __instance.minAcceptedTime = BetterAirship.MinDoorSwipeTime;
}

[HarmonyPatch(typeof(CrewmateGhostRole), nameof(CrewmateGhostRole.CanUse))]
public class CanUseCrew
{
    public static bool Prefix(CrewmateGhostRole __instance, ref bool __result)
    {
        if (__instance.Player.IsPostmortal() && !__instance.Player.Caught())
        {
            __result = true;
            return false;
        }

        return true;
    }
}

public static class CanUsePatch
{
    public static void Prefix(PlayerControl player, ref bool __state)
    {
        __state = false;

        if (player.IsPostmortal() && !player.Caught())
        {
            player.Data.IsDead = false;
            __state = true;
        }
    }

    public static void Postfix(PlayerControl player, ref bool __state)
    {
        if (__state)
            player.Data.IsDead = true;
    }
}
#endregion