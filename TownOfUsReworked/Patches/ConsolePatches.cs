namespace TownOfUsReworked.Patches;

#region OpenDoorConsole
[HarmonyPatch(typeof(OpenDoorConsole), nameof(OpenDoorConsole.CanUse))]
public static class OpenDoorConsoleCanUse
{
    public static void Prefix([HarmonyArgument(0)] GameData.PlayerInfo playerInfo, ref bool __state)
    {
        __state = false;
        var playerControl = playerInfo.Object;

        if (playerControl.IsPostmortal() && !playerControl.Caught() && playerInfo.IsDead)
        {
            playerInfo.IsDead = false;
            __state = true;
        }
    }

    public static void Postfix([HarmonyArgument(0)] GameData.PlayerInfo playerInfo, ref bool __state)
    {
        if (__state)
            playerInfo.IsDead = true;
    }
}

[HarmonyPatch(typeof(OpenDoorConsole), nameof(OpenDoorConsole.Use))]
public static class OpenDoorConsoleUse
{
    public static bool Prefix(OpenDoorConsole __instance)
    {
        __instance.CanUse(CustomPlayer.LocalCustom.Data, out var canUse, out _);
        CallRpc(CustomRPC.Misc, MiscRPC.DoorSyncToilet, __instance.MyDoor.Id);

        if (canUse)
            __instance.MyDoor.SetDoorway(true);

        return false;
    }
}
#endregion

#region DoorConsole
[HarmonyPatch(typeof(DoorConsole), nameof(DoorConsole.CanUse))]
public static class DoorConsoleCanUse
{
    public static void Prefix([HarmonyArgument(0)] GameData.PlayerInfo playerInfo, ref bool __state)
    {
        __state = false;
        var playerControl = playerInfo.Object;

        if (playerControl.IsPostmortal() && !playerControl.Caught() && playerInfo.IsDead)
        {
            playerInfo.IsDead = false;
            __state = true;
        }
    }

    public static void Postfix([HarmonyArgument(0)] GameData.PlayerInfo playerInfo, ref bool __state)
    {
        if (__state)
            playerInfo.IsDead = true;
    }
}

[HarmonyPatch(typeof(DoorConsole), nameof(DoorConsole.Use))]
public static class DoorConsoleUsePatch
{
    public static bool Prefix(DoorConsole __instance)
    {
        __instance.CanUse(CustomPlayer.LocalCustom.Data, out var canUse, out _);

        if (!canUse)
            return false;

        CustomPlayer.Local.NetTransform.Halt();
        var minigame = UObject.Instantiate(__instance.MinigamePrefab, Camera.main.transform);
        minigame.transform.localPosition = new(0f, 0f, -50f);

        try
        {
            minigame.Cast<IDoorMinigame>().SetDoor(__instance.MyDoor);
        } catch (InvalidCastException) {}

        minigame.Begin(null);
        return false;
    }
}
#endregion

#region Ladder
[HarmonyPatch(typeof(Ladder), nameof(Ladder.CanUse))]
public static class LadderCanUse
{
    public static void Prefix([HarmonyArgument(0)] GameData.PlayerInfo playerInfo, ref bool __state)
    {
        __state = false;
        var playerControl = playerInfo.Object;

        if (playerControl.IsPostmortal() && !playerControl.Caught() && playerInfo.IsDead)
        {
            playerInfo.IsDead = false;
            __state = true;
        }
    }

    public static void Postfix([HarmonyArgument(0)] GameData.PlayerInfo playerInfo, ref bool __state)
    {
        if (__state)
            playerInfo.IsDead = true;
    }
}

[HarmonyPatch(typeof(Ladder), nameof(Ladder.Use))]
public static class LadderUse
{
    public static bool Prefix(Ladder __instance)
    {
        var data = CustomPlayer.LocalCustom.Data;
        __instance.CanUse(data, out var flag, out var _);

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
    public static void Prefix([HarmonyArgument(0)] GameData.PlayerInfo playerInfo, ref bool __state)
    {
        __state = false;
        var playerControl = playerInfo.Object;

        if (playerControl.IsPostmortal() && !playerControl.Caught())
        {
            playerInfo.IsDead = false;
            __state = true;
        }
    }

    public static void Postfix([HarmonyArgument(0)] GameData.PlayerInfo playerInfo, ref bool __state)
    {
        if (__state)
            playerInfo.IsDead = true;
    }
}
#endregion

#region DeconControl
[HarmonyPatch(typeof(DeconControl), nameof(DeconControl.CanUse))]
public static class DeconControlUse
{
    public static void Prefix([HarmonyArgument(0)] GameData.PlayerInfo playerInfo, ref bool __state)
    {
        __state = false;
        var playerControl = playerInfo.Object;

        if (playerControl.IsPostmortal() && !playerControl.Caught() && playerInfo.IsDead)
        {
            playerInfo.IsDead = false;
            __state = true;
        }
    }

    public static void Postfix([HarmonyArgument(0)] GameData.PlayerInfo playerInfo, ref bool __state)
    {
        if (__state)
            playerInfo.IsDead = true;
    }
}
#endregion

#region global::Console
[HarmonyPatch(typeof(Console), nameof(Console.CanUse))]
public static class ConsoleCanUsePatch
{
    public static void Prefix([HarmonyArgument(0)] GameData.PlayerInfo playerInfo, ref bool __state)
    {
        __state = false;
        var playerControl = playerInfo.Object;

        if (playerControl.IsPostmortal() && !playerControl.Caught())
        {
            playerInfo.IsDead = false;
            __state = true;
        }
    }

    public static void Postfix([HarmonyArgument(0)] GameData.PlayerInfo playerInfo, ref bool __state)
    {
        if (__state)
            playerInfo.IsDead = true;
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

[HarmonyPatch(typeof(DoorCardSwipeGame), nameof(DoorCardSwipeGame.Begin))]
public static class DoorSwipePatch
{
    public static void Prefix(DoorCardSwipeGame __instance) => __instance.minAcceptedTime = CustomGameOptions.MinDoorSwipeTime;
}