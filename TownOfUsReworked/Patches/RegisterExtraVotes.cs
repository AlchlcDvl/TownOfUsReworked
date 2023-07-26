namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.HandleDisconnect), typeof(PlayerControl), typeof(DisconnectReasons))]
    public static class HandleDisconnect
    {
        public static void Prefix([HarmonyArgument(0)] PlayerControl player)
        {
            if (AmongUsClient.Instance.AmHost && Meeting)
            {
                foreach (var pol in Ability.GetAbilities<Politician>(AbilityEnum.Politician))
                {
                    var votesRegained = pol.ExtraVotes.RemoveAll(x => x == player.PlayerId);

                    if (pol.Local)
                        pol.VoteBank += votesRegained;

                    CallRpc(CustomRPC.Misc, MiscRPC.AddVoteBank, pol, votesRegained);
                }
            }

            DisconnectHandler.Disconnected.Add(player);
            ReassignPostmortals(player);
            MarkMeetingDead(player, player, false, true);

            if (!Summary.Disconnected.Any(x => x.PlayerName == player.name))
                Summary.AddSummaryInfo(player, true);
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Confirm))]
    [HarmonyPriority(Priority.First)]
    public static class Confirm
    {
        public static void Prefix(MeetingHud __instance)
        {
            foreach (var layer in PlayerLayer.LocalLayers)
                layer.ConfirmVotePrefix(__instance);
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
            var player = PlayerById(srcPlayerId);

            if (player.Is(RoleEnum.Mayor))
                Role.GetRole<Mayor>(player).Voted = suspectPlayerId;

            if (!player.Is(AbilityEnum.Politician))
                return true;

            var playerVoteArea = VoteAreaById(srcPlayerId);

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
                CustomPlayer.Local.RpcSendChatNote(srcPlayerId, ChatNoteTypes.DidVote);
            }

            __instance.Cast<InnerNetObject>().SetDirtyBit(1U);
            __instance.CheckForEndVoting();
            return false;
        }
    }
}