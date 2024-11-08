namespace TownOfUsReworked.Patches;

#region OpenDoorConsole
[HarmonyPatch(typeof(OpenDoorConsole), nameof(OpenDoorConsole.CanUse))]
public static class OpenDoorConsoleCanUse
{
    public static void Prefix(NetworkedPlayerInfo pc, ref bool __state) => CanUsePatch.Prefix(pc.Object, ref __state);

    public static void Postfix(NetworkedPlayerInfo pc, ref bool __state) => CanUsePatch.Postfix(pc.Object, ref __state);
}

[HarmonyPatch(typeof(OpenDoorConsole), nameof(OpenDoorConsole.Use))]
public static class OpenDoorConsoleUse
{
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
[HarmonyPatch(typeof(Console), nameof(Console.CanUse))]
public static class ConsoleCanUsePatch
{
    public static bool Prefix(Console __instance, NetworkedPlayerInfo pc, ref float __result)
    {
        var playerControl = pc.Object;
        var flag = !playerControl.CanDoTasks();

        // If the console is not a sabotage repair console
        if (flag && !__instance.AllowImpostor)
        {
            __result = float.MaxValue;
            return false;
        }

        return true;
    }

    public static void Prefix(NetworkedPlayerInfo pc, ref bool __state) => CanUsePatch.Prefix(pc.Object, ref __state);

    public static void Postfix(NetworkedPlayerInfo pc, ref bool __state) => CanUsePatch.Postfix(pc.Object, ref __state);
}
#endregion

#region ZiplineConsole
[HarmonyPatch(typeof(ZiplineConsole), nameof(ZiplineConsole.CanUse))]
public static class ZiplineConsoleCanUse
{
    public static void Prefix(NetworkedPlayerInfo pc, ref bool __state) => CanUsePatch.Prefix(pc.Object, ref __state);

    public static void Postfix(NetworkedPlayerInfo pc, ref bool __state) => CanUsePatch.Postfix(pc.Object, ref __state);
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CheckUseZipline))]
public static class PlayerControlCheckUseZipline
{
    public static void Prefix(PlayerControl target, ref bool __state) => CanUsePatch.Prefix(target, ref __state);

    public static void Postfix(PlayerControl target, ref bool __state) => CanUsePatch.Postfix(target, ref __state);
}

[HarmonyPatch(typeof(ZiplineBehaviour), nameof(ZiplineBehaviour.Use), typeof(PlayerControl), typeof(bool))]
public class ZiplineBehaviourUse
{
    public static void Prefix(PlayerControl player, ref bool __state) => CanUsePatch.Prefix(player, ref __state);

    public static void Postfix(PlayerControl player, ref bool __state) => CanUsePatch.Postfix(player, ref __state);
}
#endregion

#region Other
[HarmonyPatch(typeof(DoorCardSwipeGame), nameof(DoorCardSwipeGame.Begin))]
public static class DoorSwipePatch
{
    public static void Prefix(DoorCardSwipeGame __instance) => __instance.minAcceptedTime = BetterAirship.MinDoorSwipeTime;
}

[HarmonyPatch(typeof(Console), nameof(Console.SetOutline))]
public static class SetTaskOutline
{
    public static void Postfix(Console __instance, bool mainTarget)
    {
        if (!Role.LocalRole || !(CustomPlayer.Local && !Meeting() && CustomPlayer.Local.CanDoTasks()))
            return;

        __instance.Image.material.SetColor("_OutlineColor", Role.LocalRole.Color);
        __instance.Image.material.SetColor("_AddColor", mainTarget ? Role.LocalRole.Color : UColor.clear);
    }
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