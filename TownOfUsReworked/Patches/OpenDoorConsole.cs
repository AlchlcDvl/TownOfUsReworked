using System;
using HarmonyLib;
using UnityEngine;
using Object = UnityEngine.Object;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Data;
using Hazel;
using TownOfUsReworked.PlayerLayers.Roles;

namespace TownOfUsReworked.Patches
{
    #region OpenDoorConsole
    [HarmonyPatch(typeof(OpenDoorConsole), nameof(OpenDoorConsole.CanUse))]
    public static class OpenDoorConsoleCanUse
    {
        public static void Prefix([HarmonyArgument(0)] GameData.PlayerInfo playerInfo, ref bool __state)
        {
            __state = false;
            var playerControl = playerInfo.Object;

            if ((playerControl.Is(RoleEnum.Phantom) && !Role.GetRole<Phantom>(playerControl).Caught) || (playerControl.Is(RoleEnum.Revealer) && playerInfo.IsDead &&
                !Role.GetRole<Revealer>(playerControl).Caught) || (playerControl.Is(RoleEnum.Ghoul) && !Role.GetRole<Ghoul>(playerControl).Caught) ||
                (playerControl.Is(RoleEnum.Banshee) && !Role.GetRole<Banshee>(playerControl).Caught))
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
            __instance.CanUse(PlayerControl.LocalPlayer.Data, out var canUse, out _);

            if (!canUse)
                return false;

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

            if ((playerControl.Is(RoleEnum.Phantom) && !Role.GetRole<Phantom>(playerControl).Caught) || (playerControl.Is(RoleEnum.Revealer) &&
                !Role.GetRole<Revealer>(playerControl).Caught && playerInfo.IsDead) || (playerControl.Is(RoleEnum.Ghoul) && !Role.GetRole<Ghoul>(playerControl).Caught) ||
                (playerControl.Is(RoleEnum.Banshee) && !Role.GetRole<Banshee>(playerControl).Caught))
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
            __instance.CanUse(PlayerControl.LocalPlayer.Data, out var canUse, out _);

            if (!canUse)
                return false;

            PlayerControl.LocalPlayer.NetTransform.Halt();
            var minigame = Object.Instantiate(__instance.MinigamePrefab, Camera.main.transform);
            minigame.transform.localPosition = new Vector3(0f, 0f, -50f);

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

            if ((playerControl.Is(RoleEnum.Phantom) && !Role.GetRole<Phantom>(playerControl).Caught) || (playerControl.Is(RoleEnum.Revealer) &&
                !Role.GetRole<Revealer>(playerControl).Caught && playerInfo.IsDead))
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
            var data = PlayerControl.LocalPlayer.Data;
            __instance.CanUse(data, out var flag, out var _);

            if (flag)
                PlayerControl.LocalPlayer.MyPhysics.RpcClimbLadder(__instance);

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

            if ((playerControl.Is(RoleEnum.Phantom) && !Role.GetRole<Phantom>(playerControl).Caught) || (playerControl.Is(RoleEnum.Revealer) &&
                !Role.GetRole<Revealer>(playerControl).Caught && playerInfo.IsDead) || (playerControl.Is(RoleEnum.Ghoul) && !Role.GetRole<Ghoul>(playerControl).Caught) ||
                (playerControl.Is(RoleEnum.Banshee) && !Role.GetRole<Banshee>(playerControl).Caught))
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

            if ((playerControl.Is(RoleEnum.Phantom) && !Role.GetRole<Phantom>(playerControl).Caught) || (playerControl.Is(RoleEnum.Revealer) &&
                !Role.GetRole<Revealer>(playerControl).Caught && playerInfo.IsDead) || (playerControl.Is(RoleEnum.Ghoul) && !Role.GetRole<Ghoul>(playerControl).Caught) ||
                (playerControl.Is(RoleEnum.Banshee) && !Role.GetRole<Banshee>(playerControl).Caught))
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

            if ((playerControl.Is(RoleEnum.Phantom) && !Role.GetRole<Phantom>(playerControl).Caught) || (playerControl.Is(RoleEnum.Revealer) &&
                !Role.GetRole<Revealer>(playerControl).Caught && playerInfo.IsDead) || (playerControl.Is(RoleEnum.Ghoul) && !Role.GetRole<Ghoul>(playerControl).Caught) ||
                (playerControl.Is(RoleEnum.Banshee) && !Role.GetRole<Banshee>(playerControl).Caught))
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
            __instance.CanUse(PlayerControl.LocalPlayer.Data, out var canUse, out _);

            if (canUse)
            {
                var playerTask = __instance.FindTask(PlayerControl.LocalPlayer);

                if (playerTask.MinigamePrefab)
                {
                    var minigame = Object.Instantiate(playerTask.GetMinigamePrefab());
                    minigame.transform.SetParent(Camera.main.transform, false);
                    minigame.transform.localPosition = new Vector3(0f, 0f, -50f);
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

    [HarmonyPatch(typeof(OpenDoorConsole), nameof(OpenDoorConsole.Use))]
    public static class SyncToiletDoor
    {
        public static void Prefix(OpenDoorConsole __instance)
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.DoorSyncToilet, SendOption.Reliable);
            writer.Write(__instance.MyDoor.Id);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }
    }
}