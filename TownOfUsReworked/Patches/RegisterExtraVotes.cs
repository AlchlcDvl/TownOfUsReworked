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
                pol.VoteBank += votesRegained;
                CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, pol, PoliticianActionsRPC.Add, votesRegained);
            }
        }

        if (pc == CustomPlayer.Local)
        {
            MCIUtils.RemoveAllPlayers();
            DebuggerBehaviour.Instance.ControllingFigure = 0;
        }

        CustomPlayer.AllCustomPlayers.RemoveAll(x => x.Player == player2 || !x.Player);
        DisconnectHandler.Disconnected.Add(pc.PlayerId);
        SetPostmortals.RemoveFromPostmortals(pc);
        MarkMeetingDead(pc, false, true);
        OnGameEndPatches.AddSummaryInfo(pc, true);
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
                player.GetLayer<Politician>().ExtraVotes.Add(suspectPlayerId);
        }
        else
        {
            playerVoteArea.SetVote(suspectPlayerId);
            playerVoteArea.Flag.enabled = true;
            CustomPlayer.Local.RpcSendChatNote(srcPlayerId, ChatNoteTypes.DidVote);
        }

        __instance.SetDirtyBit(1u);
        __instance.CheckForEndVoting();
        return false;
    }
}

[HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.PopulateResults))]
public static class PopulateResults
{
    public static bool Prefix(MeetingHud __instance, ref Il2CppStructArray<MeetingHud.VoterState> states)
    {
        var allNums = new Dictionary<int, int>();
        __instance.TitleText.text = TranslationController.Instance.GetString(StringNames.MeetingVotingResults);
        var amountOfSkippedVoters = 0;

        for (var i = 0; i < __instance.playerStates.Length; i++)
        {
            var playerVoteArea = __instance.playerStates[i];
            playerVoteArea.ClearForResults();
            allNums.Add(i, 0);

            for (var stateIdx = 0; stateIdx < states.Length; stateIdx++)
            {
                var voteState = states[stateIdx];
                var playerInfo = GameData.Instance.GetPlayerById(voteState.VoterId);

                if (playerInfo == null)
                    LogError($"Couldn't find player info for voter: {voteState.VoterId}");
                else if (i == 0 && voteState.SkippedVote)
                {
                    __instance.BloopAVoteIcon(playerInfo, amountOfSkippedVoters, __instance.SkippedVoting.transform);
                    amountOfSkippedVoters++;
                }
                else if (voteState.VotedForId == playerVoteArea.TargetPlayerId)
                {
                    __instance.BloopAVoteIcon(playerInfo, allNums[i], playerVoteArea.transform);
                    allNums[i]++;
                }
            }
        }

        foreach (var politician in PlayerLayer.GetLayers<Politician>())
        {
            var playerInfo = politician.Data;
            TownOfUsReworked.NormalOptions.AnonymousVotes = GameModifiers.AnonymousVoting is AnonVotes.PoliticianOnly or AnonVotes.Enabled;

            foreach (var extraVote in politician.ExtraVotes)
            {
                if (extraVote == PlayerVoteArea.HasNotVoted || extraVote == PlayerVoteArea.MissedVote || extraVote == PlayerVoteArea.DeadVote)
                    continue;

                if (extraVote == PlayerVoteArea.SkippedVote)
                {
                    __instance.BloopAVoteIcon(playerInfo, amountOfSkippedVoters, __instance.SkippedVoting.transform);
                    amountOfSkippedVoters++;
                }
                else
                {
                    for (var i = 0; i < __instance.playerStates.Length; i++)
                    {
                        var area = __instance.playerStates[i];

                        if (extraVote != area.TargetPlayerId)
                            continue;

                        __instance.BloopAVoteIcon(playerInfo, allNums[i], area.transform);
                        allNums[i]++;
                    }
                }
            }

            TownOfUsReworked.NormalOptions.AnonymousVotes = GameModifiers.AnonymousVoting != AnonVotes.Disabled;
        }

        foreach (var mayor in PlayerLayer.GetLayers<Mayor>())
        {
            var playerInfo = mayor.Data;
            var voterArea = VoteAreaById(mayor.PlayerId);

            if (voterArea.VotedFor == PlayerVoteArea.HasNotVoted || voterArea.VotedFor == PlayerVoteArea.MissedVote || voterArea.VotedFor == PlayerVoteArea.DeadVote)
                continue;

            for (var j = 0; j < Mayor.MayorVoteCount; j++)
            {
                if (voterArea.VotedFor == PlayerVoteArea.SkippedVote)
                {
                    __instance.BloopAVoteIcon(playerInfo, amountOfSkippedVoters, __instance.SkippedVoting.transform);
                    amountOfSkippedVoters++;
                }
                else
                {
                    for (var i = 0; i < __instance.playerStates.Length; i++)
                    {
                        var area = __instance.playerStates[i];

                        if (voterArea.VotedFor != area.TargetPlayerId)
                            continue;

                        __instance.BloopAVoteIcon(playerInfo, allNums[i], area.transform);
                        allNums[i]++;
                    }
                }
            }
        }

        var alreadyKnighted = new List<byte>();

        foreach (var mon in PlayerLayer.GetLayers<Monarch>())
        {
            foreach (var id in mon.Knighted)
            {
                if (alreadyKnighted.Contains(id))
                    continue;

                alreadyKnighted.Add(id);
                var playerInfo = PlayerById(id).Data;
                var voterArea = VoteAreaById(id);

                if (voterArea.VotedFor == PlayerVoteArea.HasNotVoted || voterArea.VotedFor == PlayerVoteArea.MissedVote || voterArea.VotedFor == PlayerVoteArea.DeadVote)
                    continue;

                for (var j = 0; j < Monarch.KnightVoteCount; j++)
                {
                    if (voterArea.VotedFor == PlayerVoteArea.SkippedVote)
                    {
                        __instance.BloopAVoteIcon(playerInfo, amountOfSkippedVoters, __instance.SkippedVoting.transform);
                        amountOfSkippedVoters++;
                    }
                    else
                    {
                        for (var i = 0; i < __instance.playerStates.Length; i++)
                        {
                            var area = __instance.playerStates[i];

                            if (voterArea.VotedFor != area.TargetPlayerId)
                                continue;

                            __instance.BloopAVoteIcon(playerInfo, allNums[i], area.transform);
                            allNums[i]++;
                        }
                    }
                }
            }
        }

        return false;
    }
}

[HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.BloopAVoteIcon))]
public static class PatchVoteBloops
{
    public static bool Prefix(MeetingHud __instance, ref NetworkedPlayerInfo voterPlayer, ref int index, ref Transform parent)
    {
        var insiderFlag = CustomPlayer.Local.Is(LayerEnum.Insider) && Role.LocalRole.TasksDone;
        var deadFlag = GameModifiers.DeadSeeEverything && CustomPlayer.LocalCustom.Dead;

        if (GameModifiers.AnonymousVoting == AnonVotes.NotVisible && !(deadFlag || insiderFlag))
            return false;

        var spriteRenderer = UObject.Instantiate(__instance.PlayerVotePrefab, parent);
        spriteRenderer.transform.localScale = Vector3.zero;

        if (parent.TryGetComponent<PlayerVoteArea>(out var voteArea))
            spriteRenderer.material.SetInt(PlayerMaterial.MaskLayer, voteArea.MaskLayer);

        if (TownOfUsReworked.NormalOptions.AnonymousVotes && !(deadFlag || insiderFlag))
            PlayerMaterial.SetColors(Palette.DisabledGrey, spriteRenderer);
        else
            PlayerMaterial.SetColors(voterPlayer.DefaultOutfit.ColorId, spriteRenderer);

        __instance.StartCoroutine(Effects.Bloop(index * 0.3f, spriteRenderer.transform));
        parent.GetComponent<VoteSpreader>().AddVote(spriteRenderer);
        return false;
    }
}