namespace TownOfUsReworked.Patches;

public static class MeetingPatches
{
    private static GameData.PlayerInfo VoteTarget;
    public static int MeetingCount;
    private static GameData.PlayerInfo Reported;
    public static bool GivingAnnouncements;

    [HarmonyPatch(typeof(PlayerVoteArea), nameof(PlayerVoteArea.SetCosmetics))]
    public static class PlayerStates
    {
        public static void Postfix(PlayerVoteArea __instance)
        {
            if (CustomGameOptions.CamouflagedMeetings && HudUpdate.IsCamoed)
            {
                __instance.Background.sprite = ShipStatus.Instance.CosmeticsCache.GetNameplate("nameplate_NoPlate").Image;
                __instance.LevelNumberText.GetComponentInParent<SpriteRenderer>().enabled = false;
                __instance.LevelNumberText.GetComponentInParent<SpriteRenderer>().gameObject.SetActive(false);
            }
            else
            {
                if (ClientGameOptions.WhiteNameplates)
                    __instance.Background.sprite = ShipStatus.Instance.CosmeticsCache.GetNameplate("nameplate_NoPlate").Image;

                if (ClientGameOptions.NoLevels)
                {
                    __instance.LevelNumberText.GetComponentInParent<SpriteRenderer>().enabled = false;
                    __instance.LevelNumberText.GetComponentInParent<SpriteRenderer>().gameObject.SetActive(false);
                }
            }
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdReportDeadBody))]
    [HarmonyPriority(Priority.First)]
    public static class SetReported
    {
        public static void Postfix([HarmonyArgument(0)] GameData.PlayerInfo target) => Reported = target;
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    [HarmonyPriority(Priority.First)]
    public static class MeetingHUD_Start
    {
        public static void Postfix(MeetingHud __instance)
        {
            __instance.gameObject.AddComponent<MeetingHudPagingBehaviour>().Menu = __instance;
            OtherButtonsPatch.CloseMenus();
            MeetingCount++;
            Coroutines.Start(Announcements());
            GivingAnnouncements = true;
            CallRpc(CustomRPC.Misc, MiscRPC.MeetingStart);
            CustomPlayer.AllPlayers.ForEach(x => x?.MyPhysics?.ResetAnimState());
            AllBodies.ForEach(x => x?.gameObject?.Destroy());

            if (Role.ChaosDriveMeetingTimerCount < CustomGameOptions.ChaosDriveMeetingCount)
                Role.ChaosDriveMeetingTimerCount++;

            if ((Role.ChaosDriveMeetingTimerCount == CustomGameOptions.ChaosDriveMeetingCount || IsKilling) && !Role.SyndicateHasChaosDrive)
            {
                Role.SyndicateHasChaosDrive = true;
                RoleGen.AssignChaosDrive();
            }

            if (CachedFirstDead)
                CachedFirstDead = null;
        }

        private static IEnumerator Announcements()
        {
            yield return new WaitForSeconds(5f);

            if (CustomGameOptions.GameAnnouncements)
            {
                PlayerControl check = null;

                if (Reported != null)
                {
                    var player = Reported.Object;
                    check = player;
                    var report = $"{player.name} was found dead last round.";
                    Run(HUD.Chat, "<color=#6C29ABFF>》 Game Announcement 《</color>", report);
                    yield return new WaitForSeconds(2f);

                    if (CustomGameOptions.LocationReports)
                        report = $"Their body was found in {BodyLocations[Reported.PlayerId]}.";
                    else
                        report = "It is unknown where they died.";

                    Run(HUD.Chat, "<color=#6C29ABFF>》 Game Announcement 《</color>", report);
                    yield return new WaitForSeconds(2f);
                    var killer = PlayerById(KilledPlayers.Find(x => x.PlayerId == player.PlayerId).KillerId);
                    var killerRole = Role.GetRole(killer);

                    if (CustomGameOptions.KillerReports == RoleFactionReports.Role)
                        report = $"They were killed by the <b>{killerRole}</b>.";
                    else if (CustomGameOptions.KillerReports == RoleFactionReports.Faction)
                        report = $"They were killed by the <b>{killerRole.FactionName}</b>.";
                    else
                        report = "They were killed by an unknown assailant.";

                    Run(HUD.Chat, "<color=#6C29ABFF>》 Game Announcement 《</color>", report);
                    yield return new WaitForSeconds(2f);
                    var role = Role.GetRole(player);

                    if (CustomGameOptions.RoleFactionReports == RoleFactionReports.Role)
                        report = $"They were the <b>{role}</b>.";
                    else if (CustomGameOptions.RoleFactionReports == RoleFactionReports.Faction)
                        report = $"They were the <b>{role.FactionName}</b>.";
                    else
                        report = $"We could not determine what {player.name} was.";

                    Run(HUD.Chat, "<color=#6C29ABFF>》 Game Announcement 《</color>", report);
                }
                else
                    Run(HUD.Chat, "<color=#6C29ABFF>》 Game Announcement 《</color>", "A meeting has been called.");

                yield return new WaitForSeconds(2f);

                foreach (var player in RecentlyKilled)
                {
                    if (player != check)
                    {
                        var report = $"{player.name} was found dead last round.";
                        Run(HUD.Chat, "<color=#6C29ABFF>》 Game Announcement 《</color>", report);
                        yield return new WaitForSeconds(2f);
                        report = "It is unknown where they died.";
                        Run(HUD.Chat, "<color=#6C29ABFF>》 Game Announcement 《</color>", report);
                        yield return new WaitForSeconds(2f);

                        var killer = PlayerById(KilledPlayers.Find(x => x.PlayerId == player.PlayerId).KillerId);
                        var killerRole = Role.GetRole(killer);

                        if (Role.Cleaned.Contains(player))
                            report = "They were killed by an unknown assailant.";
                        else if (CustomGameOptions.KillerReports == RoleFactionReports.Role)
                            report = $"They were killed by the <b>{killerRole.Name}</b>.";
                        else if (CustomGameOptions.KillerReports == RoleFactionReports.Faction)
                            report = $"They were killed by the <b>{killerRole.FactionName}</b>.";
                        else
                            report = "They were killed by an unknown assailant.";

                        Run(HUD.Chat, "<color=#6C29ABFF>》 Game Announcement 《</color>", report);
                        yield return new WaitForSeconds(2f);
                        var role = Role.GetRole(player);

                        if (Role.Cleaned.Contains(player))
                            report = $"We could not determine what {player.name} was.";
                        else if (CustomGameOptions.RoleFactionReports == RoleFactionReports.Role)
                            report = $"They were the <b>{role}</b>.";
                        else if (CustomGameOptions.RoleFactionReports == RoleFactionReports.Faction)
                            report = $"They were the <b>{role.FactionName}</b>.";
                        else
                            report = $"We could not determine what {player.name} was.";

                        Run(HUD.Chat, "<color=#6C29ABFF>》 Game Announcement 《</color>", report);
                        yield return new WaitForSeconds(2f);
                    }
                }

                foreach (var player in DisconnectHandler.Disconnected)
                {
                    if (player != check)
                    {
                        Run(HUD.Chat, "<color=#6C29ABFF>》 Game Announcement 《</color>", $"{player.name} killed themselves last round.");
                        yield return new WaitForSeconds(2f);
                    }
                }

                foreach (var player in SetPostmortals.EscapedPlayers)
                {
                    if (player != check)
                    {
                        Run(HUD.Chat, "<color=#6C29ABFF>》 Game Announcement 《</color>", $"{player.name} escaped last round.");
                        yield return new WaitForSeconds(2f);
                    }
                }
            }

            var message = "";

            if (Role.ChaosDriveMeetingTimerCount < CustomGameOptions.ChaosDriveMeetingCount - 1)
                message = $"{CustomGameOptions.ChaosDriveMeetingCount - Role.ChaosDriveMeetingTimerCount} meetings remain till the <b>Syndicate</b> gets their hands on the Chaos Drive!";
            else if (Role.ChaosDriveMeetingTimerCount == CustomGameOptions.ChaosDriveMeetingCount - 1)
                message = "This is the last meeting before the <b>Syndicate</b> gets their hands on the Chaos Drive!";
            else if (Role.ChaosDriveMeetingTimerCount == CustomGameOptions.ChaosDriveMeetingCount && MeetingCount == Role.ChaosDriveMeetingTimerCount)
                message = "The <b>Syndicate</b> now possesses the Chaos Drive!";
            else
                message = "The <b>Syndicate</b> possesses the Chaos Drive.";

            Run(HUD.Chat, "<color=#6C29ABFF>》 Game Announcement 《</color>", message);

            yield return new WaitForSeconds(2f);

            if (Objectifier.GetObjectifiers(LayerEnum.Overlord).Any(x => x.IsAlive))
            {
                if (MeetingCount == CustomGameOptions.OverlordMeetingWinCount - 1)
                    message = "This is the last meeting to find and kill the <b>Overlord</b>. Should you fail, you will all lose!";
                else if (MeetingCount < CustomGameOptions.OverlordMeetingWinCount - 1)
                    message = "There seems to be an <b>Overlord</b> bent on dominating the mission! Kill them before they are successful!";

                if (message != "")
                    Run(HUD.Chat, "<color=#6C29ABFF>》 Game Announcement 《</color>", message);

                yield return new WaitForSeconds(2f);
            }

            var knighted = new List<byte>();

            foreach (var monarch in Role.GetRoles<Monarch>(LayerEnum.Monarch))
            {
                foreach (var id in monarch.ToBeKnighted)
                {
                    monarch.Knighted.Add(id);

                    if (!knighted.Contains(id))
                    {
                        var knight = PlayerById(id);

                        if (!knight.HasDied())
                        {
                            message = $"{knight.name} was knighted by a <b>Monarch</b>!";
                            Run(HUD.Chat, "<color=#6C29ABFF>》 Game Announcement 《</color>", message);
                            knighted.Add(id);
                        }
                    }

                    yield return new WaitForSeconds(2f);
                }

                monarch.ToBeKnighted.Clear();
            }

            RecentlyKilled.Clear();
            Role.Cleaned.Clear();
            GivingAnnouncements = false;
            Reported = null;
            DisconnectHandler.Disconnected.Clear();

            foreach (var layer in PlayerLayer.LocalLayers)
            {
                layer?.OnBodyReport(Reported);
                layer?.OnMeetingStart(Meeting);

                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Close))]
    public static class MeetingHud_Close
    {
        public static void Postfix(MeetingHud __instance)
        {
            PlayerLayer.LocalLayers.ForEach(x => x?.OnMeetingEnd(__instance));
            CachedFirstDead = null;
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.StartMeeting))]
    public static class StartMeetingPatch
    {
        public static void Prefix([HarmonyArgument(0)] GameData.PlayerInfo meetingTarget)
        {
            VoteTarget = meetingTarget;

            if (CustomPlayer.Local.Is(LayerEnum.Astral) && !CustomPlayer.Local.inMovingPlat && !CustomPlayer.Local.onLadder)
                Modifier.GetModifier<Astral>(CustomPlayer.Local).LastPosition = CustomPlayer.LocalCustom.Position;
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
    [HarmonyPriority(Priority.First)]
    public static class MeetingHudUpdatePatch
    {
        public static void Postfix(MeetingHud __instance)
        {
            //Deactivate skip Button if skipping on emergency meetings is disabled
            __instance.SkipVoteButton.gameObject.SetActive(!((VoteTarget == null && CustomGameOptions.SkipButtonDisable == DisableSkipButtonMeetings.Emergency) ||
                (CustomGameOptions.SkipButtonDisable == DisableSkipButtonMeetings.Always)) && __instance.state == MeetingHud.VoteStates.NotVoted &&
                !Ability.GetAssassins().Any(x => x.Phone != null) && !Role.GetRoles<Guesser>(LayerEnum.Guesser).Any(x => x.Phone != null));

            AllVoteAreas.ForEach(x => x.SetName());
            PlayerLayer.LocalLayers.ForEach(x => x?.UpdateMeeting(__instance));
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.VotingComplete))]
    public static class VotingComplete
    {
        public static void Postfix(MeetingHud __instance, [HarmonyArgument(1)] GameData.PlayerInfo exiled, [HarmonyArgument(2)] bool tie)
        {
            var exiledString = exiled == null ? "null" : exiled.PlayerName;
            LogInfo($"Exiled PlayerName = {exiledString}");
            LogInfo($"Was a tie = {tie}");
            PlayerLayer.LocalLayers.ForEach(x => x?.VoteComplete(__instance));
            Coroutines.Start(PerformSwaps());
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Select))]
    public static class MeetingHudSelect
    {
        public static void Postfix(MeetingHud __instance, int __0) => PlayerLayer.LocalLayers.ForEach(x => x?.SelectVote(__instance, __0));
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.ClearVote))]
    public static class MeetingHudClearVote
    {
        public static void Postfix(MeetingHud __instance) => PlayerLayer.LocalLayers.ForEach(x => x?.ClearVote(__instance));
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

            if ((playerControl.IsAssassin() || playerControl.Is(LayerEnum.Guesser) || playerControl.Is(LayerEnum.Thief)) && playerInfo.IsDead)
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
                var self = __instance.CalculateAllVotes(out var tie, out var maxIdx);
                var array = new Il2CppStructArray<MeetingHud.VoterState>(__instance.playerStates.Length);
                var exiled = GameData.Instance.AllPlayers.ToArray().FirstOrDefault(v => !tie && v.PlayerId == maxIdx.Key);

                for (var i = 0; i < __instance.playerStates.Length; i++)
                {
                    var playerVoteArea = __instance.playerStates[i];

                    array[i] = new MeetingHud.VoterState()
                    {
                        VoterId = playerVoteArea.TargetPlayerId,
                        VotedForId = playerVoteArea.VotedFor
                    };
                }

                __instance.RpcVotingComplete(array, exiled, tie);
                Ability.GetAbilities<Politician>(LayerEnum.Politician).ForEach(x => CallRpc(CustomRPC.Action, ActionsRPC.LayerAction2, x, PoliticianActionsRPC.Remove,
                    x.ExtraVotes.ToArray()));
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.PopulateResults))]
    public static class PopulateResults
    {
        public static bool Prefix(MeetingHud __instance, [HarmonyArgument(0)] Il2CppStructArray<MeetingHud.VoterState> states)
        {
            var allNums = new Dictionary<int, int>();
            __instance.TitleText.text = TranslationController.Instance.GetString(StringNames.MeetingVotingResults, Array.Empty<Il2CppSystem.Object>());
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
                        Debug.LogError(string.Format("Couldn't find player info for voter: {0}", voteState.VoterId));
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

            foreach (var politician in Ability.GetAbilities<Politician>(LayerEnum.Politician))
            {
                var playerInfo = politician.Player.Data;
                TownOfUsReworked.NormalOptions.AnonymousVotes = CustomGameOptions.PoliticianAnonymous;

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

                TownOfUsReworked.NormalOptions.AnonymousVotes = CustomGameOptions.AnonymousVoting;
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.BloopAVoteIcon))]
    public static class DeadSeeVoteColorsPatch
    {
        public static bool Prefix(MeetingHud __instance, [HarmonyArgument(0)] GameData.PlayerInfo voterPlayer, [HarmonyArgument(1)] int index, [HarmonyArgument(2)] Transform parent)
        {
            var spriteRenderer = UObject.Instantiate(__instance.PlayerVotePrefab);
            var insiderFlag = false;
            var deadFlag = CustomGameOptions.DeadSeeEverything && CustomPlayer.LocalCustom.IsDead;

            if (CustomPlayer.Local.Is(LayerEnum.Insider))
                insiderFlag = Role.LocalRole.TasksDone;

            if (TownOfUsReworked.NormalOptions.AnonymousVotes && !(deadFlag || insiderFlag))
                PlayerMaterial.SetColors(Palette.DisabledGrey, spriteRenderer);
            else
                PlayerMaterial.SetColors(voterPlayer.DefaultOutfit.ColorId, spriteRenderer);

            spriteRenderer.transform.SetParent(parent);
            spriteRenderer.transform.localScale = Vector3.zero;
            __instance.StartCoroutine(Effects.Bloop(index * 0.3f, spriteRenderer.transform, 1f, 0.5f));
            parent.GetComponent<VoteSpreader>().AddVote(spriteRenderer);
            return false;
        }
    }

    private static void SetName(this PlayerVoteArea player) => (player.NameText.text, player.NameText.color) = UpdateGameName(player);

    private static (string, Color) UpdateGameName(PlayerVoteArea player)
    {
        var color = UColor.white;
        var name = UpdateNames.PlayerNames.FirstOrDefault(x => x.Key == player.TargetPlayerId).Value;
        var info = player.AllPlayerInfo();
        var localinfo = CustomPlayer.Local.AllPlayerInfo();
        var roleRevealed = false;

        if (!info[0] || !localinfo[0])
            return (name, color);

        if (player.CanDoTasks() && (CustomPlayer.Local.PlayerId == player.TargetPlayerId || DeadSeeEverything))
        {
            var role = info[0] as Role;
            name = $"{name} ({role.TasksCompleted}/{role.TotalTasks})";
            roleRevealed = true;
        }

        if (player.IsKnighted())
            name += " <color=#FF004EFF>κ</color>";

        if (player.IsSpelled())
            name += " <color=#0028F5FF>ø</color>";

        if (player.IsMarked())
            name += " <color=#F1C40FFF>χ</color>";

        if (player.Is(LayerEnum.Mayor) && !DeadSeeEverything && CustomPlayer.Local.PlayerId != player.TargetPlayerId)
        {
            var mayor = info[0] as Mayor;

            if (mayor.Revealed)
            {
                roleRevealed = true;
                name += $"\n{mayor.Name}";
                color = mayor.Color;

                if (CustomPlayer.Local.Is(LayerEnum.Consigliere))
                {
                    var consigliere = localinfo[0] as Consigliere;

                    if (consigliere.Investigated.Contains(player.TargetPlayerId))
                        consigliere.Investigated.Remove(player.TargetPlayerId);
                }
                else if (CustomPlayer.Local.Is(LayerEnum.PromotedGodfather))
                {
                    var godfather = localinfo[0] as PromotedGodfather;

                    if (godfather.Investigated.Contains(player.TargetPlayerId))
                        godfather.Investigated.Remove(player.TargetPlayerId);
                }
                else if (CustomPlayer.Local.Is(LayerEnum.Inspector))
                {
                    var inspector = localinfo[0] as Inspector;

                    if (inspector.Inspected.Contains(player.TargetPlayerId))
                        inspector.Inspected.Remove(player.TargetPlayerId);
                }
                else if (CustomPlayer.Local.Is(LayerEnum.Retributionist))
                {
                    var retributionist = localinfo[0] as Retributionist;

                    if (retributionist.Inspected.Contains(player.TargetPlayerId))
                        retributionist.Inspected.Remove(player.TargetPlayerId);
                }
            }
        }
        else if (player.Is(LayerEnum.Dictator) && !DeadSeeEverything && CustomPlayer.Local.PlayerId != player.TargetPlayerId)
        {
            var dict = info[0] as Dictator;

            if (dict.Revealed)
            {
                roleRevealed = true;
                name += $"\n{dict.Name}";
                color = dict.Color;

                if (CustomPlayer.Local.Is(LayerEnum.Consigliere))
                {
                    var consigliere = localinfo[0] as Consigliere;

                    if (consigliere.Investigated.Contains(player.TargetPlayerId))
                        consigliere.Investigated.Remove(player.TargetPlayerId);
                }
                else if (CustomPlayer.Local.Is(LayerEnum.PromotedGodfather))
                {
                    var godfather = localinfo[0] as PromotedGodfather;

                    if (godfather.Investigated.Contains(player.TargetPlayerId))
                        godfather.Investigated.Remove(player.TargetPlayerId);
                }
                else if (CustomPlayer.Local.Is(LayerEnum.Inspector))
                {
                    var inspector = localinfo[0] as Inspector;

                    if (inspector.Inspected.Contains(player.TargetPlayerId))
                        inspector.Inspected.Remove(player.TargetPlayerId);
                }
                else if (CustomPlayer.Local.Is(LayerEnum.Retributionist))
                {
                    var retributionist = localinfo[0] as Retributionist;

                    if (retributionist.Inspected.Contains(player.TargetPlayerId))
                        retributionist.Inspected.Remove(player.TargetPlayerId);
                }
            }
        }

        if (CustomPlayer.Local.Is(LayerEnum.Coroner) && !DeadSeeEverything)
        {
            var coroner = localinfo[0] as Coroner;

            if (coroner.Reported.Contains(player.TargetPlayerId) && !roleRevealed)
            {
                var role = info[0] as Role;
                color = role.Color;
                name += $"\n{role}";
                roleRevealed = true;
            }
        }
        else if (CustomPlayer.Local.Is(LayerEnum.Consigliere) && !DeadSeeEverything)
        {
            var consigliere = localinfo[0] as Consigliere;

            if (consigliere.Investigated.Contains(player.TargetPlayerId) && !roleRevealed)
            {
                var role = info[0] as Role;
                roleRevealed = true;

                if (CustomGameOptions.ConsigInfo == ConsigInfo.Role)
                {
                    color = role.Color;
                    name += $"\n{role}";

                    if (consigliere.Player.GetSubFaction() == player.GetSubFaction() && player.GetSubFaction() != SubFaction.None)
                        consigliere.Investigated.Remove(player.TargetPlayerId);
                }
                else if (CustomGameOptions.ConsigInfo == ConsigInfo.Faction)
                {
                    color = role.FactionColor;
                    name += $"\n{role.FactionName}";
                }
            }
        }
        else if (CustomPlayer.Local.Is(LayerEnum.PromotedGodfather) && !DeadSeeEverything)
        {
            var godfather = localinfo[0] as PromotedGodfather;

            if (godfather.IsConsig && godfather.Investigated.Contains(player.TargetPlayerId) && !roleRevealed)
            {
                var role = info[0] as Role;
                roleRevealed = true;

                if (CustomGameOptions.ConsigInfo == ConsigInfo.Role)
                {
                    color = role.Color;
                    name += $"\n{role}";

                    if (godfather.Player.GetSubFaction() == player.GetSubFaction() && player.GetSubFaction() != SubFaction.None)
                        godfather.Investigated.Remove(player.TargetPlayerId);
                }
                else if (CustomGameOptions.ConsigInfo == ConsigInfo.Faction)
                {
                    color = role.FactionColor;
                    name += $"\n{role.FactionName}";
                }
            }
        }
        else if (CustomPlayer.Local.Is(LayerEnum.Medic))
        {
            var medic = localinfo[0] as Medic;

            if (medic.ShieldedPlayer != null && medic.ShieldedPlayer.PlayerId == player.TargetPlayerId && (int)CustomGameOptions.ShowShielded is 1 or 2)
                name += " <color=#006600FF>✚</color>";
        }
        else if (CustomPlayer.Local.Is(LayerEnum.Retributionist))
        {
            var ret = localinfo[0] as Retributionist;

            if (ret.Inspected.Contains(player.TargetPlayerId))
            {
                name += $"\n{player.GetInspResults()}";
                color = ret.Color;
                roleRevealed = true;
            }
            else if (ret.Reported.Contains(player.TargetPlayerId) && !roleRevealed)
            {
                var role = info[0] as Role;
                color = role.Color;
                name += $"\n{role}";
                roleRevealed = true;
            }
        }
        else if (CustomPlayer.Local.Is(LayerEnum.Arsonist) && !DeadSeeEverything)
        {
            var arsonist = localinfo[0] as Arsonist;

            if (arsonist.Doused.Contains(player.TargetPlayerId))
                name += " <color=#EE7600FF>Ξ</color>";
        }
        else if (CustomPlayer.Local.Is(LayerEnum.Plaguebearer) && !DeadSeeEverything)
        {
            var plaguebearer = localinfo[0] as Plaguebearer;

            if (plaguebearer.Infected.Contains(player.TargetPlayerId) && CustomPlayer.Local.PlayerId != player.TargetPlayerId)
                name += " <color=#CFFE61FF>ρ</color>";
        }
        else if (CustomPlayer.Local.Is(LayerEnum.Cryomaniac) && !DeadSeeEverything)
        {
            var cryomaniac = localinfo[0] as Cryomaniac;

            if (cryomaniac.Doused.Contains(player.TargetPlayerId))
                name += " <color=#642DEAFF>λ</color>";
        }
        else if (CustomPlayer.Local.Is(LayerEnum.Framer) && !DeadSeeEverything)
        {
            var framer = localinfo[0] as Framer;

            if (framer.Framed.Contains(player.TargetPlayerId))
                name += " <color=#00FFFFFF>ς</color>";
        }
        else if (CustomPlayer.Local.Is(LayerEnum.Executioner) && !DeadSeeEverything)
        {
            var executioner = localinfo[0] as Executioner;

            if (player.TargetPlayerId == executioner.TargetPlayer.PlayerId)
            {
                name += " <color=#CCCCCCFF>§</color>";

                if (CustomGameOptions.ExeKnowsTargetRole && !roleRevealed)
                {
                    var role = info[0] as Role;
                    color = role.Color;
                    name += $"\n{role}";
                    roleRevealed = true;
                }
                else
                    color = executioner.Color;
            }
        }
        else if (CustomPlayer.Local.Is(LayerEnum.Guesser) && !DeadSeeEverything)
        {
            var guesser = localinfo[0] as Guesser;

            if (player.TargetPlayerId == guesser.TargetPlayer.PlayerId)
            {
                color = guesser.Color;
                name += " <color=#EEE5BEFF>π</color>";
            }
        }
        else if (CustomPlayer.Local.Is(LayerEnum.GuardianAngel) && !DeadSeeEverything)
        {
            var guardianAngel = localinfo[0] as GuardianAngel;

            if (player.TargetPlayerId == guardianAngel.TargetPlayer.PlayerId)
            {
                name += " <color=#FFFFFFFF>★</color>";

                if (CustomGameOptions.GAKnowsTargetRole && !roleRevealed)
                {
                    var role = info[0] as Role;
                    color = role.Color;
                    name += $"\n{role}";
                    roleRevealed = true;
                }
                else
                    color = guardianAngel.Color;
            }
        }
        else if (CustomPlayer.Local.Is(LayerEnum.Whisperer) && !DeadSeeEverything)
        {
            var whisperer = localinfo[0] as Whisperer;

            if (whisperer.Persuaded.Contains(player.TargetPlayerId))
            {
                if (CustomGameOptions.FactionSeeRoles && !roleRevealed)
                {
                    var role = info[0] as Role;
                    color = role.Color;
                    name += $" <color=#F995FCFF>Λ</color>\n{role}";
                    roleRevealed = true;
                }
                else
                    color = whisperer.SubFactionColor;
            }
            else
            {
                foreach (var (key, value) in whisperer.PlayerConversion)
                {
                    if (player.TargetPlayerId == key)
                        name += $" {value}%";
                }
            }
        }
        else if (CustomPlayer.Local.Is(LayerEnum.Dracula) && !DeadSeeEverything)
        {
            var dracula = localinfo[0] as Dracula;

            if (dracula.Converted.Contains(player.TargetPlayerId))
            {
                if (CustomGameOptions.FactionSeeRoles && !roleRevealed)
                {
                    var role = info[0] as Role;
                    color = role.Color;
                    name += $" <color=#7B8968FF>γ</color>\n{role}";
                    roleRevealed = true;
                }
                else
                    color = dracula.SubFactionColor;
            }
        }
        else if (CustomPlayer.Local.Is(LayerEnum.Jackal) && !DeadSeeEverything)
        {
            var jackal = localinfo[0] as Jackal;

            if (jackal.Recruited.Contains(player.TargetPlayerId))
            {
                if (CustomGameOptions.FactionSeeRoles && !roleRevealed)
                {
                    var role = info[0] as Role;
                    color = role.Color;
                    name += $" <color=#575657FF>$</color>\n{role}";
                    roleRevealed = true;
                }
                else
                    color = jackal.SubFactionColor;
            }
        }
        else if (CustomPlayer.Local.Is(LayerEnum.Necromancer) && !DeadSeeEverything)
        {
            var necromancer = localinfo[0] as Necromancer;

            if (necromancer.Resurrected.Contains(player.TargetPlayerId))
            {
                if (CustomGameOptions.FactionSeeRoles && !roleRevealed)
                {
                    var role = info[0] as Role;
                    color = role.Color;
                    name += $" <color=#E6108AFF>Σ</color>\n{role}";
                    roleRevealed = true;
                }
                else
                    color = necromancer.SubFactionColor;
            }
        }
        else if (CustomPlayer.Local.Is(Alignment.NeutralKill) && !DeadSeeEverything && CustomGameOptions.NKsKnow)
        {
            if ((player.GetRole() == CustomPlayer.Local.GetRole() && CustomGameOptions.NoSolo == NoSolo.SameNKs) || (player.GetAlignment() == CustomPlayer.Local.GetAlignment() &&
                CustomGameOptions.NoSolo == NoSolo.AllNKs) && !roleRevealed)
            {
                var role = info[0] as Role;
                color = role.Color;
                name += $"\n{role}";
                roleRevealed = true;
            }
        }
        else if (CustomPlayer.Local.Is(LayerEnum.Inspector))
        {
            var inspector = localinfo[0] as Inspector;

            if (inspector.Inspected.Contains(player.TargetPlayerId) && !roleRevealed)
            {
                name += $"\n{player.GetInspResults()}";
                color = inspector.Color;
                roleRevealed = true;
            }
        }

        if (CustomPlayer.Local.IsBitten() && !DeadSeeEverything)
        {
            var dracula = CustomPlayer.Local.GetDracula();

            if (dracula.Converted.Contains(player.TargetPlayerId) && !dracula.Local)
            {
                if (CustomGameOptions.FactionSeeRoles && !roleRevealed)
                {
                    var role = info[0] as Role;
                    color = role.Color;
                    name += $" <color=#7B8968FF>γ</color>\n{role}";
                    roleRevealed = true;

                    if (CustomPlayer.Local.Is(LayerEnum.Consigliere))
                    {
                        var consigliere = localinfo[0] as Consigliere;

                        if (consigliere.Investigated.Contains(player.TargetPlayerId))
                            consigliere.Investigated.Remove(player.TargetPlayerId);
                    }
                    else if (CustomPlayer.Local.Is(LayerEnum.PromotedGodfather))
                    {
                        var godfather = localinfo[0] as PromotedGodfather;

                        if (godfather.Investigated.Contains(player.TargetPlayerId))
                            godfather.Investigated.Remove(player.TargetPlayerId);
                    }
                    else if (CustomPlayer.Local.Is(LayerEnum.Inspector))
                    {
                        var inspector = localinfo[0] as Inspector;

                        if (inspector.Inspected.Contains(player.TargetPlayerId))
                            inspector.Inspected.Remove(player.TargetPlayerId);
                    }
                    else if (CustomPlayer.Local.Is(LayerEnum.Retributionist))
                    {
                        var retributionist = localinfo[0] as Retributionist;

                        if (retributionist.Inspected.Contains(player.TargetPlayerId))
                            retributionist.Inspected.Remove(player.TargetPlayerId);
                    }
                }
                else
                    color = dracula.SubFactionColor;
            }
        }
        else if (CustomPlayer.Local.IsRecruit() && !DeadSeeEverything)
        {
            var jackal = CustomPlayer.Local.GetJackal();

            if (jackal.Recruited.Contains(player.TargetPlayerId) && !jackal.Local)
            {
                if (CustomGameOptions.FactionSeeRoles && !roleRevealed)
                {
                    var role = info[0] as Role;
                    color = role.Color;
                    name += $" <color=#575657FF>$</color>\n{role}";
                    roleRevealed = true;

                    if (CustomPlayer.Local.Is(LayerEnum.Consigliere))
                    {
                        var consigliere = localinfo[0] as Consigliere;

                        if (consigliere.Investigated.Contains(player.TargetPlayerId))
                            consigliere.Investigated.Remove(player.TargetPlayerId);
                    }
                    else if (CustomPlayer.Local.Is(LayerEnum.PromotedGodfather))
                    {
                        var godfather = localinfo[0] as PromotedGodfather;

                        if (godfather.Investigated.Contains(player.TargetPlayerId))
                            godfather.Investigated.Remove(player.TargetPlayerId);
                    }
                    else if (CustomPlayer.Local.Is(LayerEnum.Inspector))
                    {
                        var inspector = localinfo[0] as Inspector;

                        if (inspector.Inspected.Contains(player.TargetPlayerId))
                            inspector.Inspected.Remove(player.TargetPlayerId);
                    }
                    else if (CustomPlayer.Local.Is(LayerEnum.Retributionist))
                    {
                        var retributionist = localinfo[0] as Retributionist;

                        if (retributionist.Inspected.Contains(player.TargetPlayerId))
                            retributionist.Inspected.Remove(player.TargetPlayerId);
                    }
                }
                else
                    color = jackal.SubFactionColor;
            }
        }
        else if (CustomPlayer.Local.IsResurrected() && !DeadSeeEverything)
        {
            var necromancer = CustomPlayer.Local.GetNecromancer();

            if (necromancer.Resurrected.Contains(player.TargetPlayerId) && !necromancer.Local)
            {
                if (CustomGameOptions.FactionSeeRoles && !roleRevealed)
                {
                    var role = info[0] as Role;
                    color = role.Color;
                    name += $" <color=#E6108AFF>Σ</color>\n{role}";
                    roleRevealed = true;

                    if (CustomPlayer.Local.Is(LayerEnum.Consigliere))
                    {
                        var consigliere = localinfo[0] as Consigliere;

                        if (consigliere.Investigated.Contains(player.TargetPlayerId))
                            consigliere.Investigated.Remove(player.TargetPlayerId);
                    }
                    else if (CustomPlayer.Local.Is(LayerEnum.PromotedGodfather))
                    {
                        var godfather = localinfo[0] as PromotedGodfather;

                        if (godfather.Investigated.Contains(player.TargetPlayerId))
                            godfather.Investigated.Remove(player.TargetPlayerId);
                    }
                    else if (CustomPlayer.Local.Is(LayerEnum.Inspector))
                    {
                        var inspector = localinfo[0] as Inspector;

                        if (inspector.Inspected.Contains(player.TargetPlayerId))
                            inspector.Inspected.Remove(player.TargetPlayerId);
                    }
                    else if (CustomPlayer.Local.Is(LayerEnum.Retributionist))
                    {
                        var retributionist = localinfo[0] as Retributionist;

                        if (retributionist.Inspected.Contains(player.TargetPlayerId))
                            retributionist.Inspected.Remove(player.TargetPlayerId);
                    }
                }
                else
                    color = necromancer.SubFactionColor;
            }
        }
        else if (CustomPlayer.Local.IsPersuaded() && !DeadSeeEverything)
        {
            var whisperer = CustomPlayer.Local.GetWhisperer();

            if (whisperer.Persuaded.Contains(player.TargetPlayerId) && !whisperer.Local)
            {
                if (CustomGameOptions.FactionSeeRoles && !roleRevealed)
                {
                    var role = info[0] as Role;
                    color = role.Color;
                    name += $" <color=#F995FCFF>Λ</color>\n{role}";
                    roleRevealed = true;

                    if (CustomPlayer.Local.Is(LayerEnum.Consigliere))
                    {
                        var consigliere = localinfo[0] as Consigliere;

                        if (consigliere.Investigated.Contains(player.TargetPlayerId))
                            consigliere.Investigated.Remove(player.TargetPlayerId);
                    }
                    else if (CustomPlayer.Local.Is(LayerEnum.PromotedGodfather))
                    {
                        var godfather = localinfo[0] as PromotedGodfather;

                        if (godfather.Investigated.Contains(player.TargetPlayerId))
                            godfather.Investigated.Remove(player.TargetPlayerId);
                    }
                    else if (CustomPlayer.Local.Is(LayerEnum.Inspector))
                    {
                        var inspector = localinfo[0] as Inspector;

                        if (inspector.Inspected.Contains(player.TargetPlayerId))
                            inspector.Inspected.Remove(player.TargetPlayerId);
                    }
                    else if (CustomPlayer.Local.Is(LayerEnum.Retributionist))
                    {
                        var retributionist = localinfo[0] as Retributionist;

                        if (retributionist.Inspected.Contains(player.TargetPlayerId))
                            retributionist.Inspected.Remove(player.TargetPlayerId);
                    }
                }
                else
                    color = whisperer.SubFactionColor;
            }
            else
            {
                foreach (var (key, value) in whisperer.PlayerConversion)
                {
                    if (player.TargetPlayerId == key)
                        name += $" {value}%";
                }
            }
        }

        if (CustomPlayer.Local.Is(LayerEnum.Lovers) && !DeadSeeEverything)
        {
            var lover = localinfo[3] as Objectifier;
            var otherLover = CustomPlayer.Local.GetOtherLover();

            if (otherLover.PlayerId == player.TargetPlayerId)
            {
                name += $" {lover.ColoredSymbol}";

                if (CustomGameOptions.LoversRoles && !roleRevealed)
                {
                    var role = info[0] as Role;
                    color = role.Color;
                    name += $"\n{role}";
                    roleRevealed = true;

                    if (CustomPlayer.Local.Is(LayerEnum.Consigliere))
                    {
                        var consigliere = localinfo[0] as Consigliere;

                        if (consigliere.Investigated.Contains(player.TargetPlayerId))
                            consigliere.Investigated.Remove(player.TargetPlayerId);
                    }
                    else if (CustomPlayer.Local.Is(LayerEnum.PromotedGodfather))
                    {
                        var godfather = localinfo[0] as PromotedGodfather;

                        if (godfather.Investigated.Contains(player.TargetPlayerId))
                            godfather.Investigated.Remove(player.TargetPlayerId);
                    }
                    else if (CustomPlayer.Local.Is(LayerEnum.Inspector))
                    {
                        var inspector = localinfo[0] as Inspector;

                        if (inspector.Inspected.Contains(player.TargetPlayerId))
                            inspector.Inspected.Remove(player.TargetPlayerId);
                    }
                    else if (CustomPlayer.Local.Is(LayerEnum.Retributionist))
                    {
                        var retributionist = localinfo[0] as Retributionist;

                        if (retributionist.Inspected.Contains(player.TargetPlayerId))
                            retributionist.Inspected.Remove(player.TargetPlayerId);
                    }
                }
            }
        }
        else if (CustomPlayer.Local.Is(LayerEnum.Rivals) && !DeadSeeEverything)
        {
            var rival = localinfo[3] as Objectifier;
            var otherRival = CustomPlayer.Local.GetOtherRival();

            if (otherRival.PlayerId == player.TargetPlayerId)
            {
                name += $" {rival.ColoredSymbol}";

                if (CustomGameOptions.RivalsRoles && !roleRevealed)
                {
                    var role = info[0] as Role;
                    color = role.Color;
                    name += $"\n{role}";
                    roleRevealed = true;

                    if (CustomPlayer.Local.Is(LayerEnum.Consigliere))
                    {
                        var consigliere = localinfo[0] as Consigliere;

                        if (consigliere.Investigated.Contains(player.TargetPlayerId))
                            consigliere.Investigated.Remove(player.TargetPlayerId);
                    }
                    else if (CustomPlayer.Local.Is(LayerEnum.PromotedGodfather))
                    {
                        var godfather = localinfo[0] as PromotedGodfather;

                        if (godfather.Investigated.Contains(player.TargetPlayerId))
                            godfather.Investigated.Remove(player.TargetPlayerId);
                    }
                    else if (CustomPlayer.Local.Is(LayerEnum.Inspector))
                    {
                        var inspector = localinfo[0] as Inspector;

                        if (inspector.Inspected.Contains(player.TargetPlayerId))
                            inspector.Inspected.Remove(player.TargetPlayerId);
                    }
                    else if (CustomPlayer.Local.Is(LayerEnum.Retributionist))
                    {
                        var retributionist = localinfo[0] as Retributionist;

                        if (retributionist.Inspected.Contains(player.TargetPlayerId))
                            retributionist.Inspected.Remove(player.TargetPlayerId);
                    }
                }
            }
        }
        else if (CustomPlayer.Local.Is(LayerEnum.Linked) && !DeadSeeEverything)
        {
            var link = localinfo[3] as Objectifier;
            var otherLink = CustomPlayer.Local.GetOtherLink();

            if (otherLink.PlayerId == player.TargetPlayerId)
            {
                name += $" {link.ColoredSymbol}";

                if (CustomGameOptions.LinkedRoles && !roleRevealed)
                {
                    var role = info[0] as Role;
                    color = role.Color;
                    name += $"\n{role}";
                    roleRevealed = true;

                    if (CustomPlayer.Local.Is(LayerEnum.Consigliere))
                    {
                        var consigliere = localinfo[0] as Consigliere;

                        if (consigliere.Investigated.Contains(player.TargetPlayerId))
                            consigliere.Investigated.Remove(player.TargetPlayerId);
                    }
                    else if (CustomPlayer.Local.Is(LayerEnum.PromotedGodfather))
                    {
                        var godfather = localinfo[0] as PromotedGodfather;

                        if (godfather.Investigated.Contains(player.TargetPlayerId))
                            godfather.Investigated.Remove(player.TargetPlayerId);
                    }
                    else if (CustomPlayer.Local.Is(LayerEnum.Inspector))
                    {
                        var inspector = localinfo[0] as Inspector;

                        if (inspector.Inspected.Contains(player.TargetPlayerId))
                            inspector.Inspected.Remove(player.TargetPlayerId);
                    }
                    else if (CustomPlayer.Local.Is(LayerEnum.Retributionist))
                    {
                        var retributionist = localinfo[0] as Retributionist;

                        if (retributionist.Inspected.Contains(player.TargetPlayerId))
                            retributionist.Inspected.Remove(player.TargetPlayerId);
                    }
                }
            }
        }
        else if (CustomPlayer.Local.Is(LayerEnum.Mafia) && !DeadSeeEverything)
        {
            var mafia = localinfo[3] as Mafia;

            if (player.Is(LayerEnum.Mafia) && player.TargetPlayerId != CustomPlayer.Local.PlayerId)
            {
                name += $" {mafia.ColoredSymbol}";

                if (CustomGameOptions.MafiaRoles && !roleRevealed)
                {
                    var role = info[0] as Role;
                    color = role.Color;
                    name += $"\n{role}";
                    roleRevealed = true;

                    if (CustomPlayer.Local.Is(LayerEnum.Consigliere))
                    {
                        var consigliere = localinfo[0] as Consigliere;

                        if (consigliere.Investigated.Contains(player.TargetPlayerId))
                            consigliere.Investigated.Remove(player.TargetPlayerId);
                    }
                    else if (CustomPlayer.Local.Is(LayerEnum.PromotedGodfather))
                    {
                        var godfather = localinfo[0] as PromotedGodfather;

                        if (godfather.Investigated.Contains(player.TargetPlayerId))
                            godfather.Investigated.Remove(player.TargetPlayerId);
                    }
                    else if (CustomPlayer.Local.Is(LayerEnum.Inspector))
                    {
                        var inspector = localinfo[0] as Inspector;

                        if (inspector.Inspected.Contains(player.TargetPlayerId))
                            inspector.Inspected.Remove(player.TargetPlayerId);
                    }
                    else if (CustomPlayer.Local.Is(LayerEnum.Retributionist))
                    {
                        var retributionist = localinfo[0] as Retributionist;

                        if (retributionist.Inspected.Contains(player.TargetPlayerId))
                            retributionist.Inspected.Remove(player.TargetPlayerId);
                    }
                }
            }
        }

        if (CustomPlayer.Local.Is(LayerEnum.Snitch) && CustomGameOptions.SnitchSeestargetsInMeeting && !DeadSeeEverything && player.TargetPlayerId != CustomPlayer.Local.PlayerId)
        {
            var role = localinfo[0] as Role;

            if (role.TasksDone)
            {
                var role2 = info[0] as Role;

                if (CustomGameOptions.SnitchSeesRoles)
                {
                    if (player.Is(Faction.Syndicate) || player.Is(Faction.Intruder) || (player.Is(Faction.Neutral) && CustomGameOptions.SnitchSeesNeutrals) || (player.Is(Faction.Crew)
                        && CustomGameOptions.SnitchSeesCrew))
                    {
                        color = role2.Color;
                        name += $"\n{role2.Name}";
                        roleRevealed = true;
                    }
                }
                else if (player.Is(Faction.Syndicate) || player.Is(Faction.Intruder) || (player.Is(Faction.Neutral) && CustomGameOptions.SnitchSeesNeutrals) || (player.Is(Faction.Crew)
                    && CustomGameOptions.SnitchSeesCrew))
                {
                    if (!(player.Is(LayerEnum.Traitor) && CustomGameOptions.SnitchSeesTraitor) && !(player.Is(LayerEnum.Fanatic) && CustomGameOptions.SnitchSeesFanatic))
                    {
                        color = role2.FactionColor;
                        name += $"\n{role2.FactionName}";
                    }
                    else
                    {
                        color = Colors.Crew;
                        name += "\nCrew";
                    }

                    roleRevealed = true;
                }
            }
        }

        if (player.Is(LayerEnum.Snitch))
        {
            var role = info[0] as Role;

            if (role.TasksDone || role.TasksLeft <= CustomGameOptions.SnitchTasksRemaining)
            {
                var ability = info[2] as Ability;
                color = ability.Color;
                name += (name.Contains('\n') ? " " : "\n") + $"{ability.Name}";
                roleRevealed = true;
            }
        }

        if (CustomPlayer.Local.GetFaction() == player.GetFaction() && player.TargetPlayerId != CustomPlayer.Local.PlayerId && (player.GetFaction() == Faction.Intruder || player.GetFaction()
            == Faction.Syndicate) && !DeadSeeEverything)
        {
            var role = info[0] as Role;

            if (CustomGameOptions.FactionSeeRoles && !roleRevealed)
            {
                color = role.Color;
                name += $"\n{role}";
                roleRevealed = true;

                if (CustomPlayer.Local.Is(LayerEnum.Consigliere))
                {
                    var consigliere = localinfo[0] as Consigliere;

                    if (consigliere.Investigated.Contains(player.TargetPlayerId))
                        consigliere.Investigated.Remove(player.TargetPlayerId);
                }
                else if (CustomPlayer.Local.Is(LayerEnum.PromotedGodfather))
                {
                    var godfather = localinfo[0] as PromotedGodfather;

                    if (godfather.Investigated.Contains(player.TargetPlayerId))
                        godfather.Investigated.Remove(player.TargetPlayerId);
                }
            }
            else
                color = role.FactionColor;

            if (player.SyndicateSided() || player.IntruderSided())
            {
                var objectifier = info[3] as Objectifier;
                name += $" {objectifier.ColoredSymbol}";
            }
            else
                name += $" {role.FactionColorString}ξ</color>";
        }

        if ((CustomPlayer.Local.Is(Faction.Syndicate) || DeadSeeEverything) && (player.TargetPlayerId == Role.DriveHolder?.PlayerId || (CustomGameOptions.GlobalDrive &&
            Role.SyndicateHasChaosDrive && player.Is(Faction.Syndicate))))
        {
            name += " <color=#008000FF>Δ</color>";
        }

        if (Role.GetRoles<Revealer>(LayerEnum.Revealer).Any(x => x.CompletedTasks) && CustomPlayer.Local.Is(Faction.Crew))
        {
            var role = info[0] as Role;

            if (CustomGameOptions.RevealerRevealsRoles)
            {
                if (player.Is(Faction.Syndicate) || player.Is(Faction.Intruder) || (player.Is(Faction.Neutral) && CustomGameOptions.RevealerRevealsNeutrals) || (player.Is(Faction.Crew) &&
                    CustomGameOptions.RevealerRevealsCrew))
                {
                    color = role.Color;
                    name += $"\n{role}";
                    roleRevealed = true;
                }
            }
            else if (player.Is(Faction.Syndicate) || player.Is(Faction.Intruder) || (player.Is(Faction.Neutral) && CustomGameOptions.RevealerRevealsNeutrals) || (player.Is(Faction.Crew) &&
                CustomGameOptions.RevealerRevealsCrew))
            {
                if (!(player.Is(LayerEnum.Traitor) && CustomGameOptions.RevealerRevealsTraitor) && !(player.Is(LayerEnum.Fanatic) &&
                    CustomGameOptions.RevealerRevealsFanatic))
                {
                    color = role.FactionColor;
                    name += $"\n{role.FactionName}";
                }
                else
                {
                    color = Colors.Crew;
                    name += "\nCrew";
                }

                roleRevealed = true;
            }
        }

        if (player.TargetPlayerId == CustomPlayer.Local.PlayerId && !player.AmDead)
        {
            if (player.IsShielded() && (int)CustomGameOptions.ShowShielded is 0 or 2)
                name += " <color=#006600FF>✚</color>";

            if (player.IsBHTarget())
                name += " <color=#B51E39FF>Θ</color>";

            if (player.IsExeTarget() && CustomGameOptions.ExeTargetKnows)
                name += " <color=#CCCCCCFF>§</color>";

            if (player.IsGATarget() && CustomGameOptions.GATargetKnows)
                name += " <color=#FFFFFFFF>★</color>";

            if (player.IsGuessTarget() && CustomGameOptions.GuesserTargetKnows)
                name += " <color=#EEE5BEFF>π</color>";

            if (player.IsBitten())
                name += " <color=#7B8968FF>γ</color>";

            if (player.IsRecruit())
                name += " <color=#575657FF>$</color>";

            if (player.IsResurrected())
                name += " <color=#E6108AFF>Σ</color>";

            if (player.IsPersuaded())
                name += " <color=#F995FCFF>Λ</color>";
        }

        if (DeadSeeEverything)
        {
            if (player.IsShielded() && CustomGameOptions.ShowShielded != ShieldOptions.Everyone)
                name += " <color=#006600FF>✚</color>";

            if (player.IsBHTarget())
                name += " <color=#B51E39FF>Θ</color>";

            if (player.IsExeTarget())
                name += " <color=#CCCCCCFF>§</color>";

            if (player.IsGATarget())
                name += " <color=#FFFFFFFF>★</color>";

            if (player.IsGuessTarget())
                name += " <color=#EEE5BEFF>π</color>";

            if (player.IsBitten())
                name += " <color=#7B8968FF>γ</color>";

            if (player.IsRecruit())
                name += " <color=#575657FF>$</color>";

            if (player.IsResurrected())
                name += " <color=#E6108AFF>Σ</color>";

            if (player.IsPersuaded())
                name += " <color=#F995FCFF>Λ</color>";

            if (player == Role.DriveHolder)
                name += " <color=#008000FF>Δ</color>";

            if (player.IsFramed())
                name += " <color=#00FFFFFF>ς</color>";

            if (player.IsInfected())
                name += " <color=#CFFE61FF>ρ</color>";

            if (player.IsArsoDoused())
                name += " <color=#EE7600FF>Ξ</color>";

            if (player.IsCryoDoused())
                name += " <<color=#642DEAFF>λ</color>";

            if (CustomPlayer.Local.Is(LayerEnum.Consigliere))
            {
                var consigliere = localinfo[0] as Consigliere;
                consigliere.Investigated.Clear();
            }

            if (CustomPlayer.Local.Is(LayerEnum.PromotedGodfather))
            {
                var godfather = localinfo[0] as PromotedGodfather;
                godfather.Investigated.Clear();
            }

            if (CustomPlayer.Local.Is(LayerEnum.Inspector))
            {
                var inspector = localinfo[0] as Inspector;
                inspector.Inspected.Clear();
            }

            if (CustomPlayer.Local.Is(LayerEnum.Retributionist))
            {
                var retributionist = localinfo[0] as Retributionist;
                retributionist.Inspected.Clear();
            }
        }

        if (DeadSeeEverything || player.TargetPlayerId == CustomPlayer.Local.PlayerId)
        {
            if (info[3])
            {
                var objectifier = info[3] as Objectifier;

                if (objectifier.Type != LayerEnum.None && !objectifier.Hidden)
                    name += $" {objectifier.ColoredSymbol}";
            }

            if (!roleRevealed)
            {
                var role = info[0] as Role;
                color = role.Color;
                name += $"\n{role}";
                roleRevealed = true;
            }
        }

        if (roleRevealed)
            player.ColorBlindName.transform.localPosition = new(-0.93f, -0.2f, -0.1f);

        return (name, color);
    }

    private static IEnumerator Slide2D(Transform target, Vector2 source, Vector2 dest, float duration)
    {
        var temp = default(Vector3);
        temp.z = target.position.z;

        for (var time = 0f; time < duration; time += Time.deltaTime)
        {
            var t = time / duration;
            temp.x = Mathf.SmoothStep(source.x, dest.x, t);
            temp.y = Mathf.SmoothStep(source.y, dest.y, t);
            target.position = temp;
            yield return null;
        }

        temp.x = dest.x;
        temp.y = dest.y;
        target.position = temp;
    }

    private static IEnumerator PerformSwaps()
    {
        foreach (var role in Ability.GetAbilities<Swapper>(LayerEnum.Swapper))
        {
            if (role.IsDead || role.Disconnected || role.Swap1 == null || role.Swap2 == null)
                continue;

            var swapPlayer1 = PlayerByVoteArea(role.Swap1);
            var swapPlayer2 = PlayerByVoteArea(role.Swap2);

            if (swapPlayer1 == null || swapPlayer2 == null || swapPlayer1.HasDied() || swapPlayer2.HasDied())
                continue;

            var pool1 = role.Swap1.PlayerIcon.transform;
            var name1 = role.Swap1.NameText.transform;
            var background1 = role.Swap1.Background.transform;
            var mask1 = role.Swap1.MaskArea.transform;
            var whiteBackground1 = role.Swap1.PlayerButton.transform;
            var level1 = role.Swap1.LevelNumberText.transform;
            var cb1 = role.Swap1.ColorBlindName.transform;
            var overlay1 = role.Swap1.Overlay.transform;
            var report1 = role.Swap1.Megaphone.transform;

            var pooldest1 = (Vector2)pool1.position;
            var namedest1 = (Vector2)name1.position;
            var backgroundDest1 = (Vector2)background1.position;
            var whiteBackgroundDest1 = (Vector2)whiteBackground1.position;
            var maskdest1 = (Vector2)mask1.position;
            var leveldest1 = (Vector2)level1.position;
            var cbdest1 = (Vector2)cb1.position;
            var overlaydest1 = (Vector2)overlay1.position;
            var reportdest1 = (Vector2)report1.position;

            var pool2 = role.Swap2.PlayerIcon.transform;
            var name2 = role.Swap2.NameText.transform;
            var background2 = role.Swap2.Background.transform;
            var mask2 = role.Swap2.MaskArea.transform;
            var whiteBackground2 = role.Swap2.PlayerButton.transform;
            var level2 = role.Swap1.LevelNumberText.transform;
            var cb2 = role.Swap2.ColorBlindName.transform;
            var overlay2 = role.Swap2.Overlay.transform;
            var report2 = role.Swap2.Megaphone.transform;

            var pooldest2 = (Vector2)pool2.position;
            var namedest2 = (Vector2)name2.position;
            var backgrounddest2 = (Vector2)background2.position;
            var whiteBackgroundDest2 = (Vector2)whiteBackground2.position;
            var maskdest2 = (Vector2)mask2.position;
            var leveldest2 = (Vector2)level2.position;
            var cbdest2 = (Vector2)cb2.position;
            var overlaydest2 = (Vector2)overlay2.position;
            var reportdest2 = (Vector2)report2.position;

            var duration = 1f / Ability.GetAbilities(LayerEnum.Swapper).Count;

            Coroutines.Start(Slide2D(cb1, cbdest1, cbdest2, duration));
            Coroutines.Start(Slide2D(cb2, cbdest2, cbdest1, duration));
            Coroutines.Start(Slide2D(pool1, pooldest1, pooldest2, duration));
            Coroutines.Start(Slide2D(pool2, pooldest2, pooldest1, duration));
            Coroutines.Start(Slide2D(name1, namedest1, namedest2, duration));
            Coroutines.Start(Slide2D(name2, namedest2, namedest1, duration));
            Coroutines.Start(Slide2D(mask1, maskdest1, maskdest2, duration));
            Coroutines.Start(Slide2D(mask2, maskdest2, maskdest1, duration));
            Coroutines.Start(Slide2D(level1, leveldest1, leveldest2, duration));
            Coroutines.Start(Slide2D(level2, leveldest2, leveldest1, duration));
            Coroutines.Start(Slide2D(report1, reportdest1, reportdest2, duration));
            Coroutines.Start(Slide2D(report2, reportdest2, reportdest1, duration));
            Coroutines.Start(Slide2D(overlay1, overlaydest1, overlaydest2, duration));
            Coroutines.Start(Slide2D(overlay2, overlaydest2, overlaydest1, duration));
            Coroutines.Start(Slide2D(whiteBackground1, whiteBackgroundDest1, whiteBackgroundDest2, duration));
            Coroutines.Start(Slide2D(whiteBackground2, whiteBackgroundDest2, whiteBackgroundDest1, duration));

            yield return new WaitForSeconds(duration);
        }
    }
}