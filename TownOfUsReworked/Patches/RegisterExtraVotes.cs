namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.HandleDisconnect), typeof(PlayerControl), typeof(DisconnectReasons))]
public static class HandleDisconnect
{
    public static void Prefix(ref PlayerControl pc)
    {
        var player2 = pc;

        if (AmongUsClient.Instance.AmHost && Meeting)
        {
            foreach (var pol in PlayerLayer.GetLayers<Politician>())
            {
                var votesRegained = pol.ExtraVotes.RemoveAll(x => x == player2.PlayerId);

                if (pol.Local)
                    pol.VoteBank += votesRegained;

                CallRpc(CustomRPC.Action, ActionsRPC.LayerAction2, pol, PoliticianActionsRPC.Add, votesRegained);
            }
        }

        CustomPlayer.AllCustomPlayers.RemoveAll(x => x.Player == player2 || x.Player == null);
        DisconnectHandler.Disconnected.Add(pc);
        ReassignPostmortals(pc);
        MarkMeetingDead(pc, pc, false, true);
        OnGameEndPatch.AddSummaryInfo(pc, true);

        if (!OnGameEndPatch.Disconnected.Any(x => x.PlayerName == player2.name))
            OnGameEndPatch.AddSummaryInfo(pc, true);
    }
}

[HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Confirm))]
public static class Confirm
{
    public static void Prefix(MeetingHud __instance) => PlayerLayer.LocalLayers.ForEach(x => x?.ConfirmVotePrefix(__instance));

    public static void Postfix(MeetingHud __instance) => PlayerLayer.LocalLayers.ForEach(x => x?.ConfirmVotePostfix(__instance));
}

[HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.CastVote))]
public static class CastVote
{
    public static bool Prefix(MeetingHud __instance, ref byte srcPlayerId, ref byte suspectPlayerId)
    {
        var player = PlayerById(srcPlayerId);

        if (!player.Is(LayerEnum.Politician))
            return true;

        var playerVoteArea = VoteAreaById(srcPlayerId);

        if (playerVoteArea.AmDead)
            return false;

        if (playerVoteArea.DidVote)
        {
            if (player.Is(LayerEnum.Politician))
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