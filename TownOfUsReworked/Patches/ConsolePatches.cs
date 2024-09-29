namespace TownOfUsReworked.Patches;

#region OpenDoorConsole
[HarmonyPatch(typeof(OpenDoorConsole), nameof(OpenDoorConsole.CanUse))]
public static class OpenDoorConsoleCanUse
{
    public static void Prefix(NetworkedPlayerInfo pc, ref bool __state)
    {
        __state = false;
        var playerControl = pc.Object;

        if (playerControl.IsPostmortal() && !playerControl.Caught())
        {
            pc.IsDead = false;
            __state = true;
        }
    }

    public static void Postfix(NetworkedPlayerInfo pc, ref bool __state)
    {
        if (__state)
            pc.IsDead = true;
    }
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
    public static void Prefix(NetworkedPlayerInfo pc, ref bool __state)
    {
        __state = false;
        var playerControl = pc.Object;

        if (playerControl.IsPostmortal() && !playerControl.Caught())
        {
            pc.IsDead = false;
            __state = true;
        }
    }

    public static void Postfix(NetworkedPlayerInfo pc, ref bool __state)
    {
        if (__state)
            pc.IsDead = true;
    }
}

[HarmonyPatch(typeof(DoorConsole), nameof(DoorConsole.Use))]
public static class DoorConsoleUsePatch
{
    public static bool Prefix(DoorConsole __instance)
    {
        __instance.CanUse(CustomPlayer.LocalCustom.Data, out var canUse, out _);

        if (canUse)
        {
            CustomPlayer.Local.NetTransform.Halt();
            var minigame = UObject.Instantiate(__instance.MinigamePrefab, Camera.main.transform);
            minigame.transform.localPosition = new(0f, 0f, -50f);
            minigame.TryCast<IDoorMinigame>()?.SetDoor(__instance.MyDoor);
            minigame.Begin(null);
        }

        return false;
    }
}
#endregion

#region Ladder
[HarmonyPatch(typeof(Ladder), nameof(Ladder.CanUse))]
public static class LadderCanUse
{
    public static void Prefix(NetworkedPlayerInfo pc, ref bool __state)
    {
        __state = false;
        var playerControl = pc.Object;

        if (playerControl.IsPostmortal() && !playerControl.Caught())
        {
            pc.IsDead = false;
            __state = true;
        }
    }

    public static void Postfix(NetworkedPlayerInfo pc, ref bool __state)
    {
        if (__state)
            pc.IsDead = true;
    }
}

[HarmonyPatch(typeof(Ladder), nameof(Ladder.Use))]
public static class LadderUse
{
    public static bool Prefix(Ladder __instance)
    {
        var data = CustomPlayer.LocalCustom.Data;
        __instance.CanUse(data, out var flag, out _);

        if (flag)
            CustomPlayer.Local.MyPhysics.RpcClimbLadder(__instance);

        return false;
    }
}
#endregion

#region PlatformConsole
[HarmonyPatch(typeof(PlatformConsole), nameof(PlatformConsole.CanUse))]
public static class PlatformConsoleCanUse
{
    public static void Prefix(NetworkedPlayerInfo pc, ref bool __state)
    {
        __state = false;
        var playerControl = pc.Object;

        if (playerControl.IsPostmortal() && !playerControl.Caught())
        {
            pc.IsDead = false;
            __state = true;
        }
    }

    public static void Postfix(NetworkedPlayerInfo pc, ref bool __state)
    {
        if (__state)
            pc.IsDead = true;
    }
}

[HarmonyPatch(typeof(PlatformConsole), nameof(PlatformConsole.Use))]
public static class PlatformConsoleUse
{
    public static bool Prefix(PlatformConsole __instance)
    {
        var data = PlayerControl.LocalPlayer.Data;
        __instance.CanUse(data, out var flag, out var _);

        if (flag)
            __instance.Platform.Use();

        return false;
    }
}

[HarmonyPatch(typeof(MovingPlatformBehaviour), nameof(MovingPlatformBehaviour.Use), typeof(PlayerControl))]
public static class MovingPlatformBehaviourUse
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

#region DeconControl
[HarmonyPatch(typeof(DeconControl), nameof(DeconControl.CanUse))]
public static class DeconControlUse
{
    public static void Prefix(NetworkedPlayerInfo pc, ref bool __state)
    {
        __state = false;
        var playerControl = pc.Object;

        if (playerControl.IsPostmortal() && !playerControl.Caught())
        {
            pc.IsDead = false;
            __state = true;
        }
    }

    public static void Postfix(NetworkedPlayerInfo pc, ref bool __state)
    {
        if (__state)
            pc.IsDead = true;
    }
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

    public static void Prefix(NetworkedPlayerInfo pc, ref bool __state)
    {
        __state = false;
        var playerControl = pc.Object;

        if (playerControl.IsPostmortal() && !playerControl.Caught())
        {
            pc.IsDead = false;
            __state = true;
        }
    }

    public static void Postfix(NetworkedPlayerInfo pc, ref bool __state)
    {
        if (__state)
            pc.IsDead = true;
    }
}

[HarmonyPatch(typeof(Console), nameof(Console.Use))]
public static class ConsoleUsePatch
{
    public static bool Prefix(Console __instance)
    {
        __instance.CanUse(CustomPlayer.LocalCustom.Data, out var canUse, out _);

        if (canUse)
        {
            var playerTask = __instance.FindTask(CustomPlayer.Local);

            if (playerTask.MinigamePrefab)
            {
                var minigame = UObject.Instantiate(playerTask.GetMinigamePrefab());
                minigame.transform.SetParent(Camera.main.transform, false);
                minigame.transform.localPosition = new(0f, 0f, -50f);
                minigame.Console = __instance;
                minigame.Begin(playerTask);
            }
        }

        return false;
    }
}
#endregion

#region ZiplineConsole
[HarmonyPatch(typeof(ZiplineConsole), nameof(ZiplineConsole.CanUse))]
public static class ZiplineConsoleCanUse
{
    public static void Prefix(NetworkedPlayerInfo pc, ref bool __state)
    {
        __state = false;
        var playerControl = pc.Object;

        if (playerControl.IsPostmortal() && !playerControl.Caught())
        {
            pc.IsDead = false;
            __state = true;
        }
    }

    public static void Postfix([HarmonyArgument(0)] NetworkedPlayerInfo playerInfo, ref bool __state)
    {
        if (__state)
            playerInfo.IsDead = true;
    }
}

[HarmonyPatch(typeof(ZiplineConsole), nameof(ZiplineConsole.Use))]
public static class ZiplineConsoleUse
{
    public static bool Prefix(ZiplineConsole __instance)
    {
        var data = PlayerControl.LocalPlayer.Data;
        __instance.CanUse(data, out var flag, out var _);

        if (flag)
            __instance.zipline.Use(__instance.atTop, __instance);

        return false;
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CheckUseZipline))]
public static class PlayerControlCheckUseZipline
{
    public static void Prefix(PlayerControl target, ref bool __state)
    {
        var targetData = target.CachedPlayerData;
        __state = false;

        if (target.IsPostmortal() && !target.Caught())
        {
            targetData.IsDead = false;
            __state = true;
        }
    }

    public static void Postfix(PlayerControl target, ref bool __state)
    {
        var targetData = target.CachedPlayerData;

        if (__state)
            targetData.IsDead = true;
    }
}

[HarmonyPatch(typeof(ZiplineBehaviour), nameof(ZiplineBehaviour.Use), typeof(PlayerControl), typeof(bool))]
public class ZiplineBehaviourUse
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
#endregion