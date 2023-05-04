using System;
using HarmonyLib;
using Hazel;
using TownOfUsReworked.Classes;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;
using System.Collections.Generic;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.PlayerLayers;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(AirshipExileController), nameof(AirshipExileController.WrapUpAndSpawn))]
    public static class AirshipExile
    {
        public static void Postfix(AirshipExileController __instance) => SetPostmortals.ExileControllerPostfix(__instance);
    }

    [HarmonyPatch(typeof(Object), nameof(Object.Destroy), new Type[] { typeof(GameObject) })]
    public static class SubmergedExile
    {
        public static void Prefix(GameObject obj)
        {
            if (!SubmergedCompatibility.Loaded || GameOptionsManager.Instance.currentNormalGameOptions.MapId != 5)
                return;

            if (obj.name.Contains("ExileCutscene"))
                SetPostmortals.ExileControllerPostfix(ConfirmEjects.LastExiled);
        }
    }

    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    public static class SetPostmortals
    {
        public readonly static List<PlayerControl> AssassinatedPlayers = new();

        public static void Postfix(ExileController __instance) => ExileControllerPostfix(__instance);

        public static void ExileControllerPostfix(ExileController __instance)
        {
            foreach (var player in AssassinatedPlayers)
            {
                if (!player.Data.Disconnected)
                    player.Exiled();
            }

            AssassinatedPlayers.Clear();

            foreach (var ghoul in Role.GetRoles<Ghoul>(RoleEnum.Ghoul))
            {
                if (ghoul.Caught)
                    ghoul.MarkedPlayer = null;
                else if (ghoul.MarkedPlayer != null && !(ghoul.MarkedPlayer.Data.IsDead || ghoul.MarkedPlayer.Data.Disconnected))
                    ghoul.MarkedPlayer.Exiled();
            }

            var exiled = __instance.exiled?.Object;

            if (exiled != null)
            {
                JesterWin(exiled);
                ExecutionerWin(exiled);

                if (exiled.Is(ObjectifierEnum.Lovers))
                {
                    var lover = exiled.GetOtherLover();

                    if (!lover.Is(RoleEnum.Pestilence) && CustomGameOptions.BothLoversDie)
                        lover?.Exiled();
                }

                Reassign(exiled);
            }

            foreach (var dict in Role.GetRoles<Dictator>(RoleEnum.Dictator))
            {
                if (dict.Revealed && dict.ToBeEjected.Count > 0)
                {
                    foreach (var exiled1 in dict.ToBeEjected)
                    {
                        var player = Utils.PlayerById(exiled1);

                        if (player == null)
                            continue;

                        player.Exiled();
                        var role = Role.GetRole(player);
                        role.KilledBy = " By " + dict.PlayerName;
                        role.DeathReason = DeathReasonEnum.Dictated;
                    }

                    if (dict.ToDie)
                    {
                        dict.Player.Exiled();
                        dict.DeathReason = DeathReasonEnum.Suicide;
                    }

                    dict.Ejected = true;
                }
            }
        }

        public static void Reassign(PlayerControl player)
        {
            SetRevealer(player);
            SetPhantom(player);
            SetBanshee(player);
            SetGhoul(player);
        }

        private static void JesterWin(PlayerControl player)
        {
            foreach (var jest in Role.GetRoles<Jester>(RoleEnum.Jester))
            {
                if (jest.Player == player)
                {
                    jest.VotedOut = true;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                    writer.Write((byte)WinLoseRPC.JesterWin);
                    writer.Write(jest.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }
        }

        private static void ExecutionerWin(PlayerControl player)
        {
            foreach (var exe in Role.GetRoles<Executioner>(RoleEnum.Executioner))
            {
                if (exe.TargetPlayer == null || (!CustomGameOptions.ExeCanWinBeyondDeath && exe.IsDead))
                    continue;

                if (player == exe.TargetPlayer)
                {
                    exe.TargetVotedOut = true;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                    writer.Write((byte)WinLoseRPC.ExecutionerWin);
                    writer.Write(exe.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }
        }

        public static void SetStartingVent(PlayerControl player)
        {
            var startingVent = ShipStatus.Instance.AllVents[Random.RandomRangeInt(0, ShipStatus.Instance.AllVents.Count)];

            var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetPos, SendOption.Reliable);
            writer2.Write(player.PlayerId);
            writer2.Write(startingVent.transform.position.x);
            writer2.Write(startingVent.transform.position.y + 0.3636f);
            AmongUsClient.Instance.FinishRpcImmediately(writer2);

            player.NetTransform.RpcSnapTo(new Vector2(startingVent.transform.position.x, startingVent.transform.position.y + 0.3636f));
            player.MyPhysics.RpcEnterVent(startingVent.Id);
        }

        public static void RemoveTasks(PlayerControl player)
        {
            foreach (var task in player.myTasks)
            {
                if (task.TryCast<NormalPlayerTask>() != null)
                {
                    var normalPlayerTask = task.Cast<NormalPlayerTask>();
                    var updateArrow = normalPlayerTask.taskStep > 0;
                    normalPlayerTask.taskStep = 0;
                    normalPlayerTask.Initialize();

                    if (normalPlayerTask.TaskType == TaskTypes.PickUpTowels)
                    {
                        foreach (var console in Object.FindObjectsOfType<TowelTaskConsole>())
                            console.Image.color = Color.white;
                    }

                    normalPlayerTask.taskStep = 0;

                    if (normalPlayerTask.TaskType == TaskTypes.UploadData)
                        normalPlayerTask.taskStep = 1;

                    if ((normalPlayerTask.TaskType is TaskTypes.EmptyGarbage or TaskTypes.EmptyChute) && (GameOptionsManager.Instance.currentNormalGameOptions.MapId is 0 or 3 or 4))
                        normalPlayerTask.taskStep = 1;

                    if (updateArrow)
                        normalPlayerTask.UpdateArrow();

                    var taskInfo = player.Data.FindTaskById(task.Id);
                    taskInfo.Complete = false;
                }
            }
        }

        #pragma warning disable
        public static PlayerControl WillBeRevealer;
        public static bool RevealerOn;
        #pragma warning restore

        public static void SetRevealer(PlayerControl exiled)
        {
            if (!RevealerOn)
                return;

            if ((WillBeRevealer?.Data.IsDead == false || WillBeRevealer.Data.Disconnected) && exiled.Is(Faction.Crew))
                WillBeRevealer = exiled;

            if (WillBeRevealer?.Is(RoleEnum.Revealer) == false)
            {
                var former = Role.GetRole(WillBeRevealer);
                var role = new Revealer(WillBeRevealer) { FormerRole = former };
                role.RoleUpdate(former);
                RemoveTasks(PlayerControl.LocalPlayer);
                WillBeRevealer.gameObject.layer = LayerMask.NameToLayer("Players");

                if (PlayerControl.LocalPlayer != WillBeRevealer)
                    PlayerControl.LocalPlayer.MyPhysics.ResetMoveState();
            }

            if (WillBeRevealer == PlayerControl.LocalPlayer)
            {
                if (Role.GetRole<Revealer>(WillBeRevealer).Caught)
                    return;

                SetStartingVent(WillBeRevealer);
            }
        }

        #pragma warning disable
        public static PlayerControl WillBePhantom;
        public static bool PhantomOn;
        #pragma warning restore

        public static void SetPhantom(PlayerControl exiled)
        {
            if (!PhantomOn || LayerExtentions.NeutralHasUnfinishedBusiness(exiled))
                return;

            if ((WillBePhantom?.Data.IsDead == false || WillBePhantom.Data.Disconnected) && exiled.Is(Faction.Neutral))
                WillBePhantom = exiled;

            if (WillBePhantom?.Is(RoleEnum.Phantom) == false)
            {
                var former = Role.GetRole(WillBePhantom);
                var role = new Phantom(WillBePhantom);
                role.RoleUpdate(former);
                RemoveTasks(WillBePhantom);
                WillBePhantom.gameObject.layer = LayerMask.NameToLayer("Players");

                if (PlayerControl.LocalPlayer != WillBePhantom)
                    PlayerControl.LocalPlayer.MyPhysics.ResetMoveState();
            }

            if (WillBePhantom == PlayerControl.LocalPlayer)
            {
                if (Role.GetRole<Phantom>(WillBePhantom).Caught)
                    return;

                SetStartingVent(WillBePhantom);
            }
        }

        #pragma warning disable
        public static PlayerControl WillBeBanshee;
        public static bool BansheeOn;
        #pragma warning restore

        public static void SetBanshee(PlayerControl exiled)
        {
            if (!BansheeOn)
                return;

            if ((WillBeBanshee?.Data.IsDead == false || WillBeBanshee.Data.Disconnected) && exiled.Is(Faction.Syndicate))
                WillBeBanshee = exiled;

            if (WillBeBanshee?.Is(RoleEnum.Banshee) == false)
            {
                var former = Role.GetRole(WillBeBanshee);
                var role = new Banshee(WillBeBanshee);
                role.RoleUpdate(former);
                WillBeBanshee.gameObject.layer = LayerMask.NameToLayer("Players");

                if (PlayerControl.LocalPlayer != WillBeBanshee)
                    PlayerControl.LocalPlayer.MyPhysics.ResetMoveState();
            }

            if (WillBeBanshee == PlayerControl.LocalPlayer)
            {
                if (Role.GetRole<Banshee>(WillBeBanshee).Caught)
                    return;

                SetStartingVent(WillBeBanshee);
            }
        }

        #pragma warning disable
        public static PlayerControl WillBeGhoul;
        public static bool GhoulOn;
        #pragma warning restore

        public static void SetGhoul(PlayerControl exiled)
        {
            if (!GhoulOn)
                return;

            if ((WillBeGhoul?.Data.IsDead == false || WillBeGhoul.Data.Disconnected) && exiled.Is(Faction.Syndicate))
                WillBeGhoul = exiled;

            if (WillBeGhoul?.Is(RoleEnum.Banshee) == false)
            {
                var former = Role.GetRole(WillBeGhoul);
                var role = new Ghoul(WillBeGhoul);
                role.RoleUpdate(former);
                WillBeGhoul.gameObject.layer = LayerMask.NameToLayer("Players");

                if (PlayerControl.LocalPlayer != WillBeGhoul)
                    PlayerControl.LocalPlayer.MyPhysics.ResetMoveState();
            }

            if (WillBeGhoul == PlayerControl.LocalPlayer)
            {
                if (Role.GetRole<Ghoul>(WillBeGhoul).Caught)
                    return;

                SetStartingVent(WillBeGhoul);
            }
        }
    }
}