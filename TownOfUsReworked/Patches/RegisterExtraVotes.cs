using HarmonyLib;
using Hazel;
using InnerNet;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.PlayerLayers.Abilities;
using TownOfUsReworked.PlayerLayers;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.HandleDisconnect), typeof(PlayerControl), typeof(DisconnectReasons))]
    public static class HandleDisconnect
    {
        public static void Prefix([HarmonyArgument(0)] PlayerControl player)
        {
            if (AmongUsClient.Instance.AmHost && MeetingHud.Instance)
            {
                foreach (var pol in Ability.GetAbilities<Politician>(AbilityEnum.Politician))
                {
                    var votesRegained = pol.ExtraVotes.RemoveAll(x => x == player.PlayerId);

                    if (pol.Player == PlayerControl.LocalPlayer)
                        pol.VoteBank += votesRegained;

                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AddVoteBank, SendOption.Reliable);
                    writer.Write(pol.PlayerId);
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
            foreach (var layer in PlayerLayer.LocalLayers)
                layer.ConfirmVotePrefix(__instance);

            return true;
        }

        public static void Postfix(MeetingHud __instance)
        {
            foreach (var layer in PlayerLayer.LocalLayers)
                layer.ConfirmVotePostfix(__instance);
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.CastVote))]
    public static class CastVote
    {
        public static bool Prefix(MeetingHud __instance, [HarmonyArgument(0)] byte srcPlayerId, [HarmonyArgument(1)] byte suspectPlayerId)
        {
            var player = Utils.PlayerById(srcPlayerId);

            if (player.Is(RoleEnum.Mayor))
                Role.GetRole<Mayor>(player).Voted = suspectPlayerId;

            if (!player.Is(AbilityEnum.Politician))
                return true;

            var playerVoteArea = Utils.VoteAreaById(srcPlayerId);

            if (playerVoteArea.AmDead)
                return false;

            if (playerVoteArea.DidVote)
            {
                if (player.Is(AbilityEnum.Politician))
                    Ability.GetAbility<Politician>(player).ExtraVotes.Add(suspectPlayerId);
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