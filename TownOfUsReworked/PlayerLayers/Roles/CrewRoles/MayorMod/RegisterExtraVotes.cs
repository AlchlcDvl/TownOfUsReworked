using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Hazel;
using InnerNet;
using TownOfUsReworked.Classes;
using TownOfUsReworked.PlayerLayers.Abilities;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MayorMod
{
    [HarmonyPatch]
    public static class RegisterExtraVotes
    {
        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
        public static void Postfix(MeetingHud __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Mayor))
                return;

            if (PlayerControl.LocalPlayer.Data.IsDead)
                return;

            if (__instance.TimerText.text.Contains("Can Vote"))
                return;

            var role = Role.GetRole<Mayor>(PlayerControl.LocalPlayer);
            __instance.TimerText.text = $"Can Vote: {role.VoteBank} time(s) | {__instance.TimerText.text}";
        }

        public static Dictionary<byte, int> CalculateAllVotes(MeetingHud __instance)
        {
            var dictionary = new Dictionary<byte, int>();

            for (var i = 0; i < __instance.playerStates.Length; i++)
            {
                var playerVoteArea = __instance.playerStates[i];

                if (!playerVoteArea.DidVote || playerVoteArea.AmDead || playerVoteArea.VotedFor == PlayerVoteArea.MissedVote || playerVoteArea.VotedFor == PlayerVoteArea.DeadVote)
                    continue;

                if (dictionary.TryGetValue(playerVoteArea.VotedFor, out var num))
                    dictionary[playerVoteArea.VotedFor] = num + 1;
                else
                    dictionary[playerVoteArea.VotedFor] = 1;
            }

            foreach (var role in Role.GetRoles<Mayor>(RoleEnum.Mayor))
            {
                foreach (var number in role.ExtraVotes)
                {
                    if (dictionary.TryGetValue(number, out var num))
                        dictionary[number] = num + 1;
                    else
                        dictionary[number] = 1;
                }
            }

            dictionary.MaxPair(out var tie);

            if (tie)
            {
                foreach (var player in __instance.playerStates)
                {
                    if (!player.DidVote || player.AmDead || player.VotedFor == PlayerVoteArea.MissedVote || player.VotedFor == PlayerVoteArea.DeadVote)
                        continue;

                    var ability = Ability.GetAbility(player);

                    if (ability == null)
                        continue;

                    if (ability.AbilityType == AbilityEnum.Tiebreaker)
                    {
                        if (dictionary.TryGetValue(player.VotedFor, out var num))
                            dictionary[player.VotedFor] = num + 1;
                        else
                            dictionary[player.VotedFor] = 1;
                    }
                }
            }

            return dictionary;
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
        public static void Prefix()
        {
            foreach (var mayor in Role.GetRoles<Mayor>(RoleEnum.Mayor))
            {
                mayor.ExtraVotes.Clear();

                if (mayor.VoteBank < 0)
                    mayor.VoteBank = 0;

                mayor.VoteBank++;
                mayor.SelfVote = false;
                mayor.VotedOnce = false;
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.HandleDisconnect), typeof(PlayerControl), typeof(DisconnectReasons))]
        [HarmonyPrefix]
        public static void Prefix([HarmonyArgument(0)] PlayerControl player)
        {
            if (AmongUsClient.Instance.AmHost && MeetingHud.Instance)
            {
                foreach (var mayor in Role.GetRoles<Mayor>(RoleEnum.Mayor))
                {
                    var votesRegained = mayor.ExtraVotes.RemoveAll(x => x == player.PlayerId);

                    if (mayor.Player == PlayerControl.LocalPlayer)
                        mayor.VoteBank += votesRegained;

                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AddMayorVoteBank, SendOption.Reliable);
                    writer.Write(mayor.Player.PlayerId);
                    writer.Write(votesRegained);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Confirm))]
        public static class Confirm
        {
            public static bool Prefix(MeetingHud __instance)
            {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Mayor))
                    return true;

                if (__instance.state != MeetingHud.VoteStates.Voted)
                    return true;

                __instance.state = MeetingHud.VoteStates.NotVoted;
                return true;
            }

            [HarmonyPriority(Priority.First)]
            public static void Postfix(MeetingHud __instance)
            {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Mayor))
                    return;

                var role = Role.GetRole<Mayor>(PlayerControl.LocalPlayer);

                if (role.CanVote)
                    __instance.SkipVoteButton.gameObject.SetActive(true);
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.CastVote))]
        public static class CastVote
        {
            public static bool Prefix(MeetingHud __instance, [HarmonyArgument(0)] byte srcPlayerId, [HarmonyArgument(1)] byte suspectPlayerId)
            {
                var player = PlayerControl.AllPlayerControls.ToArray().FirstOrDefault(x => x.PlayerId == srcPlayerId);

                if (!player.Is(RoleEnum.Mayor))
                    return true;

                var playerVoteArea = __instance.playerStates.ToArray().First(pv => pv.TargetPlayerId == srcPlayerId);

                if (playerVoteArea.AmDead)
                    return false;

                var role = Role.GetRole<Mayor>(player);

                if (playerVoteArea.DidVote)
                    role.ExtraVotes.Add(suspectPlayerId);
                else
                {
                    playerVoteArea.SetVote(suspectPlayerId);
                    playerVoteArea.Flag.enabled = true;
                    PlayerControl.LocalPlayer.RpcSendChatNote(srcPlayerId, ChatNoteTypes.DidVote);
                }

                __instance.Cast<InnerNetObject>().SetDirtyBit(1U);
                __instance.CheckForEndVoting();
                return false;
            }
        }
    }
}