using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Hazel;
using TownOfUsReworked.Classes;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;
using UnityEngine.UI;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.SwapperMod
{
    [HarmonyPatch]
    public static class ShowHideSwapButtons
    {
        public static Dictionary<byte, int> CalculateVotes(MeetingHud __instance)
        {
            var self = Utils.CalculateAllVotes(__instance);

            foreach (var swapper in Role.GetRoles<Swapper>(RoleEnum.Swapper))
            {
                if (swapper.IsDead || swapper.Disconnected || swapper.Swap1 == null || swapper.Swap2 == null)
                    continue;

                PlayerControl swapPlayer1 = null;
                PlayerControl swapPlayer2 = null;

                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player.PlayerId == swapper.Swap1.TargetPlayerId)
                        swapPlayer1 = player;

                    if (player.PlayerId == swapper.Swap2.TargetPlayerId)
                        swapPlayer2 = player;
                }

                if (swapPlayer1 == null || swapPlayer2 == null || swapPlayer1.Data.IsDead || swapPlayer1.Data.Disconnected || swapPlayer2.Data.IsDead || swapPlayer2.Data.Disconnected)
                    continue;

                var swap1 = 0;
                var swap2 = 0;

                if (self.TryGetValue(swapper.Swap1.TargetPlayerId, out var value))
                    swap1 = value;

                if (self.TryGetValue(swapper.Swap2.TargetPlayerId, out var value2))
                    swap2 = value2;

                self[swapper.Swap2.TargetPlayerId] = swap1;
                self[swapper.Swap1.TargetPlayerId] = swap2;
            }

            return self;
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Confirm))]
        public static class Confirm
        {
            public static bool Prefix(MeetingHud __instance)
            {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Swapper) || CustomGameOptions.SwapAfterVoting)
                    return true;

                var swapper = Role.GetRole<Swapper>(PlayerControl.LocalPlayer);

                foreach (var button in swapper.MoarButtons.Where(button => button != null))
                {
                    if (button.GetComponent<SpriteRenderer>().sprite == AssetManager.GetSprite("SwapperSwitchDisabled"))
                        button.SetActive(false);

                    button.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
                }

                if (swapper.ListOfActives.Count(x => x) == 2)
                {
                    for (var i = 0; i < swapper.ListOfActives.Count; i++)
                    {
                        if (!swapper.ListOfActives[i])
                            continue;

                        if (swapper.Swap1 == null)
                            swapper.Swap1 = __instance.playerStates[i];
                        else
                            swapper.Swap2 = __instance.playerStates[i];
                    }
                }

                if (swapper.Swap1 == null || swapper.Swap2 == null)
                    return true;

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.SetSwaps);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write(swapper.Swap1.TargetPlayerId);
                writer.Write(swapper.Swap2.TargetPlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                return true;
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.CheckForEndVoting))]
        public static class CheckForEndVoting
        {
            private static bool CheckVoted(PlayerVoteArea playerVoteArea)
            {
                if (playerVoteArea.AmDead || playerVoteArea.DidVote)
                    return true;

                var playerInfo = GameData.Instance.GetPlayerById(playerVoteArea.TargetPlayerId);

                if (playerInfo == null)
                    return true;

                var playerControl = playerInfo.Object;

                if (playerControl.Is(AbilityEnum.Assassin) && playerInfo.IsDead)
                {
                    playerVoteArea.VotedFor = PlayerVoteArea.DeadVote;
                    playerVoteArea.SetDead(false, true);
                }

                return true;
            }

            public static bool Prefix(MeetingHud __instance)
            {
                if (__instance.playerStates.All(ps => ps.AmDead || (ps.DidVote && CheckVoted(ps))))
                {
                    var self = CalculateVotes(__instance);
                    var array = new Il2CppStructArray<MeetingHud.VoterState>(__instance.playerStates.Length);
                    var maxIdx = self.MaxPair(out var tie);
                    Utils.LogSomething($"Meeting was a tie = {tie}");
                    var exiled = GameData.Instance.AllPlayers.ToArray().FirstOrDefault(v => !tie && v.PlayerId == maxIdx.Key);

                    for (var i = 0; i < __instance.playerStates.Length; i++)
                    {
                        var playerVoteArea = __instance.playerStates[i];

                        array[i] = new MeetingHud.VoterState
                        {
                            VoterId = playerVoteArea.TargetPlayerId,
                            VotedForId = playerVoteArea.VotedFor
                        };
                    }

                    __instance.RpcVotingComplete(array, exiled, tie);

                    foreach (var role in Role.GetRoles<Mayor>(RoleEnum.Mayor))
                    {
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                        writer.Write((byte)ActionsRPC.SetExtraVotes);
                        writer.Write(role.Player.PlayerId);
                        writer.WriteBytesAndSize(role.ExtraVotes.ToArray());
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                    }

                    foreach (var role in Role.GetRoles<Politician>(RoleEnum.Politician))
                    {
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                        writer.Write((byte)ActionsRPC.SetExtraVotesPol);
                        writer.Write(role.Player.PlayerId);
                        writer.WriteBytesAndSize(role.ExtraVotes.ToArray());
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                    }

                    foreach (var role in Role.GetRoles<PromotedRebel>(RoleEnum.PromotedRebel))
                    {
                        if (!role.IsPol)
                            continue;

                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                        writer.Write((byte)ActionsRPC.SetExtraVotesReb);
                        writer.Write(role.Player.PlayerId);
                        writer.WriteBytesAndSize(role.ExtraVotes.ToArray());
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                    }
                }

                return false;
            }
        }
    }
}