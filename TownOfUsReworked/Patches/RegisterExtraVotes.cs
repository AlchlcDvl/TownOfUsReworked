using System.Linq;
using HarmonyLib;
using Hazel;
using InnerNet;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;
using TownOfUsReworked.PlayerLayers.Roles;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
    public static class UpdateMeeting
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Mayor) && !PlayerControl.LocalPlayer.Is(RoleEnum.Politician) && !PlayerControl.LocalPlayer.Is(RoleEnum.PromotedRebel))
                return;

            if (PlayerControl.LocalPlayer.Data.IsDead)
                return;

            if (__instance.TimerText.text.Contains("Can Vote"))
                return;

            var bank = -1;

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Mayor))
            {
                var role = Role.GetRole<Mayor>(PlayerControl.LocalPlayer);
                bank = role.VoteBank;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Politician))
            {
                var role = Role.GetRole<Politician>(PlayerControl.LocalPlayer);
                bank = role.VoteBank;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.PromotedRebel))
            {
                var role = Role.GetRole<PromotedRebel>(PlayerControl.LocalPlayer);

                if (!role.IsPol)
                    return;

                bank = role.VoteBank;
            }

            if (bank != -1)
                __instance.TimerText.text = $"Can Vote: {bank} time(s) | {__instance.TimerText.text}";
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.HandleDisconnect), typeof(PlayerControl), typeof(DisconnectReasons))]
    public static class HandleDisconnect
    {
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

                foreach (var politician in Role.GetRoles<Politician>(RoleEnum.Politician))
                {
                    var votesRegained = politician.ExtraVotes.RemoveAll(x => x == player.PlayerId);

                    if (politician.Player == PlayerControl.LocalPlayer)
                        politician.VoteBank += votesRegained;

                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AddPoliticianVoteBank, SendOption.Reliable);
                    writer.Write(politician.Player.PlayerId);
                    writer.Write(votesRegained);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }

                foreach (var role in Role.GetRoles<PromotedRebel>(RoleEnum.PromotedRebel))
                {
                    if (!role.IsPol)
                        continue;

                    var votesRegained = role.ExtraVotes.RemoveAll(x => x == player.PlayerId);

                    if (role.Player == PlayerControl.LocalPlayer)
                        role.VoteBank += votesRegained;

                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AddRebPoliticianVoteBank, SendOption.Reliable);
                    writer.Write(role.Player.PlayerId);
                    writer.Write(votesRegained);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }

            DisconnectHandler.Disconnected.Add(player);
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Confirm))]
    [HarmonyPriority(Priority.First)]
    public static class Confirm
    {
        public static bool Prefix(MeetingHud __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Mayor) && !PlayerControl.LocalPlayer.Is(RoleEnum.Politician) && !PlayerControl.LocalPlayer.Is(RoleEnum.PromotedRebel))
                return true;

            if (__instance.state != MeetingHud.VoteStates.Voted)
                return true;

            __instance.state = MeetingHud.VoteStates.NotVoted;
            return true;
        }

        public static void Postfix(MeetingHud __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Mayor) && !PlayerControl.LocalPlayer.Is(RoleEnum.Politician) && !PlayerControl.LocalPlayer.Is(RoleEnum.PromotedRebel))
                return;

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Mayor))
            {
                var role = Role.GetRole<Mayor>(PlayerControl.LocalPlayer);

                if (role.CanVote)
                    __instance.SkipVoteButton.gameObject.SetActive(true);
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Politician))
            {
                var role = Role.GetRole<Politician>(PlayerControl.LocalPlayer);

                if (role.CanVote)
                    __instance.SkipVoteButton.gameObject.SetActive(true);
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.PromotedRebel))
            {
                var role = Role.GetRole<PromotedRebel>(PlayerControl.LocalPlayer);

                if (role.IsPol && role.CanVote)
                    __instance.SkipVoteButton.gameObject.SetActive(true);
            }
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.CastVote))]
    public static class CastVote
    {
        public static bool Prefix(MeetingHud __instance, [HarmonyArgument(0)] byte srcPlayerId, [HarmonyArgument(1)] byte suspectPlayerId)
        {
            var player = PlayerControl.AllPlayerControls.ToArray().FirstOrDefault(x => x.PlayerId == srcPlayerId);

            if (!player.Is(RoleEnum.Mayor) && !player.Is(RoleEnum.Politician) && !player.Is(RoleEnum.PromotedRebel))
                return true;

            var playerVoteArea = __instance.playerStates.ToArray().First(pv => pv.TargetPlayerId == srcPlayerId);

            if (playerVoteArea.AmDead)
                return false;

            if (playerVoteArea.DidVote)
            {
                if (player.Is(RoleEnum.Mayor))
                    Role.GetRole<Mayor>(player).ExtraVotes.Add(suspectPlayerId);
                else if (player.Is(RoleEnum.Politician))
                    Role.GetRole<Politician>(player).ExtraVotes.Add(suspectPlayerId);
                else if (player.Is(RoleEnum.PromotedRebel))
                {
                    var role = Role.GetRole<PromotedRebel>(player);

                    if (role.IsPol)
                        role.ExtraVotes.Add(suspectPlayerId);
                }
            }
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