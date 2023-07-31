namespace TownOfUsReworked.Patches
{
    [HarmonyPatch]
    public static class MeetingPatches
    {
        private static GameData.PlayerInfo VoteTarget;
        public static int MeetingCount;
        private static GameData.PlayerInfo Reported;
        public static bool GivingAnnouncements;
        private static DeadBody ReportedBody;

        [HarmonyPatch(typeof(PlayerVoteArea), nameof(PlayerVoteArea.SetCosmetics))]
        public static class PlayerStates
        {
            public static void Postfix(PlayerVoteArea __instance)
            {
                if (CustomGameOptions.MeetingColourblind && DoUndo.IsCamoed)
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

        [HarmonyPatch(typeof(PlayerVoteArea), nameof(PlayerVoteArea.PreviewNameplate))]
        public static class PlayerPreviews
        {
            public static void Postfix(PlayerVoteArea __instance)
            {
                if (CustomGameOptions.MeetingColourblind && DoUndo.IsCamoed)
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
            public static void Postfix([HarmonyArgument(0)] GameData.PlayerInfo target)
            {
                Reported = target;
                ReportedBody = target == null ? null : BodyById(target.PlayerId);
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
        [HarmonyPriority(Priority.First)]
        public static class MeetingHUD_Start
        {
            public static void Postfix(MeetingHud __instance)
            {
                __instance.gameObject.AddComponent<MeetingHudPagingBehaviour>().Menu = __instance;

                if (OtherButtonsPatch.SettingsActive)
                    OtherButtonsPatch.OpenSettings();

                if (OtherButtonsPatch.Zooming)
                    OtherButtonsPatch.Zoom();

                if (OtherButtonsPatch.WikiActive)
                    OtherButtonsPatch.OpenWiki();

                if (OtherButtonsPatch.RoleCardActive)
                    OtherButtonsPatch.OpenRoleCard();

                MeetingCount++;
                Coroutines.Start(Announcements());
                GivingAnnouncements = true;
                CallRpc(CustomRPC.Misc, MiscRPC.MeetingStart);
                CustomPlayer.AllPlayers.ForEach(x => x?.MyPhysics?.ResetAnimState());
                AllBodies.ForEach(x => x.gameObject.Destroy());

                if (Role.ChaosDriveMeetingTimerCount < CustomGameOptions.ChaosDriveMeetingCount)
                    Role.ChaosDriveMeetingTimerCount++;

                if ((Role.ChaosDriveMeetingTimerCount == CustomGameOptions.ChaosDriveMeetingCount || IsKilling) && !Role.SyndicateHasChaosDrive)
                {
                    Role.SyndicateHasChaosDrive = true;
                    RoleGen.AssignChaosDrive();
                }
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
                        HUD.Chat.AddChat(CustomPlayer.Local, report);

                        yield return new WaitForSeconds(2f);

                        if (CustomGameOptions.LocationReports)
                            report = $"Their body was found in {GetLocation(ReportedBody.TruePosition)}.";
                        else
                            report = "It is unknown where they died.";

                        HUD.Chat.AddChat(CustomPlayer.Local, report);

                        yield return new WaitForSeconds(2f);

                        var killer = PlayerById(KilledPlayers.Find(x => x.PlayerId == player.PlayerId).KillerId);
                        var flag = killer.Is(RoleEnum.Altruist) || killer.Is(RoleEnum.Arsonist) || killer.Is(RoleEnum.Amnesiac) || killer.Is(RoleEnum.Executioner) ||
                            killer.Is(RoleEnum.Engineer) || killer.Is(RoleEnum.Escort) || killer.Is(RoleEnum.Impostor) || killer.Is(RoleEnum.Inspector) || killer.Is(RoleEnum.Operative);
                        var a_an = flag ? "an" : "a";
                        var flag1 = killer.Is(Faction.Intruder);
                        var a_an4 = flag1 ? "an" : "a";
                        var killerRole = Role.GetRole(killer);

                        if (CustomGameOptions.KillerReports == RoleFactionReports.Role)
                            report = $"They were killed by {a_an} <b>{killerRole}</b>.";
                        else if (CustomGameOptions.KillerReports == RoleFactionReports.Faction)
                            report = $"They were killed by {a_an4} <b>{killerRole.FactionName}</b>.";
                        else
                            report = "They were killed by an unknown assailant.";

                        HUD.Chat.AddChat(CustomPlayer.Local, report);

                        yield return new WaitForSeconds(2f);

                        var flag2 = player.Is(RoleEnum.Altruist) || player.Is(RoleEnum.Arsonist) || player.Is(RoleEnum.Amnesiac) || player.Is(RoleEnum.Executioner) ||
                            player.Is(RoleEnum.Engineer) || player.Is(RoleEnum.Escort) || player.Is(RoleEnum.Impostor) || player.Is(RoleEnum.Inspector) || player.Is(RoleEnum.Operative);
                        var a_an2 = flag2 ? "an" : "a";
                        var role = Role.GetRole(player);
                        var flag3 = player.Is(Faction.Intruder);
                        var a_an3 = flag3 ? "an" : "a";

                        if (CustomGameOptions.RoleFactionReports == RoleFactionReports.Role)
                            report = $"They were {a_an2} <b>{role}</b>.";
                        else if (CustomGameOptions.RoleFactionReports == RoleFactionReports.Faction)
                            report = $"They were {a_an3} <b>{role.FactionName}</b>.";
                        else
                            report = $"We could not determine what {player.name} was.";

                        HUD.Chat.AddChat(CustomPlayer.Local, report);
                    }
                    else
                        HUD.Chat.AddChat(CustomPlayer.Local, "A meeting has been called.");

                    yield return new WaitForSeconds(2f);

                    foreach (var player in RecentlyKilled)
                    {
                        if (player != check)
                        {
                            var report = $"{player.name} was found dead last round.";
                            HUD.Chat.AddChat(CustomPlayer.Local, report);

                            yield return new WaitForSeconds(2f);

                            report = "It is unknown where they died.";
                            HUD.Chat.AddChat(CustomPlayer.Local, report);

                            yield return new WaitForSeconds(2f);

                            var killer = PlayerById(KilledPlayers.Find(x => x.PlayerId == player.PlayerId).KillerId);
                            var killerRole = Role.GetRole(killer);
                            var flag = killer.Is(RoleEnum.Altruist) || killer.Is(RoleEnum.Arsonist) || killer.Is(RoleEnum.Amnesiac) || killer.Is(RoleEnum.Executioner) ||
                                killer.Is(RoleEnum.Engineer) || killer.Is(RoleEnum.Escort) || killer.Is(RoleEnum.Impostor) || killer.Is(RoleEnum.Inspector) ||
                                killer.Is(RoleEnum.Operative);
                            var a_an = flag ? "an" : "a";
                            var flag4 = killer.Is(Faction.Intruder);
                            var a_an4 = flag4 ? "an" : "a";

                            if (Role.Cleaned.Contains(player))
                                report = "They were killed by an unknown assailant.";
                            else if (CustomGameOptions.KillerReports == RoleFactionReports.Role)
                                report = $"They were killed by {a_an} <b>{killerRole.Name}</b>.";
                            else if (CustomGameOptions.KillerReports == RoleFactionReports.Faction)
                                report = $"They were killed by {a_an4} <b>{killerRole.FactionName}</b>.";
                            else
                                report = "They were killed by an unknown assailant.";

                            HUD.Chat.AddChat(CustomPlayer.Local, report);

                            yield return new WaitForSeconds(2f);

                            var role = Role.GetRole(player);
                            var flag2 = player.Is(RoleEnum.Altruist) || player.Is(RoleEnum.Arsonist) || player.Is(RoleEnum.Amnesiac) || player.Is(RoleEnum.Executioner) ||
                                player.Is(RoleEnum.Engineer) || player.Is(RoleEnum.Escort) || player.Is(RoleEnum.Impostor) || player.Is(RoleEnum.Inspector) ||
                                player.Is(RoleEnum.Operative);
                            var a_an2 = flag2 ? "an" : "a";
                            var flag3 = player.Is(Faction.Intruder);
                            var a_an3 = flag3 ? "an" : "a";

                            if (Role.Cleaned.Contains(player))
                                report = $"We could not determine what {player.name} was.";
                            else if (CustomGameOptions.RoleFactionReports == RoleFactionReports.Role)
                                report = $"They were {a_an2} <b>{role}</b>.";
                            else if (CustomGameOptions.RoleFactionReports == RoleFactionReports.Faction)
                                report = $"They were {a_an3} <b>{role.FactionName}</b>.";
                            else
                                report = $"We could not determine what {player.name} was.";

                            HUD.Chat.AddChat(CustomPlayer.Local, report);

                            yield return new WaitForSeconds(2f);
                        }
                    }

                    foreach (var player in DisconnectHandler.Disconnected)
                    {
                        if (player != check)
                        {
                            HUD.Chat.AddChat(CustomPlayer.Local, $"{player.name} escaped the ship last round.");

                            yield return new WaitForSeconds(2f);
                        }
                    }

                    foreach (var player in SetPostmortals.EscapedPlayers)
                    {
                        if (player != check)
                        {
                            HUD.Chat.AddChat(CustomPlayer.Local, $"{player.name} escaped the ship last round.");

                            yield return new WaitForSeconds(2f);
                        }
                    }
                }

                var message = "";

                if (Role.ChaosDriveMeetingTimerCount < CustomGameOptions.ChaosDriveMeetingCount - 1)
                    message = $"{CustomGameOptions.ChaosDriveMeetingCount - Role.ChaosDriveMeetingTimerCount} meetings remain till the <b>Syndicate</b> gets their hands on the Chaos Drive!";
                else if (Role.ChaosDriveMeetingTimerCount == CustomGameOptions.ChaosDriveMeetingCount - 1)
                    message = "This is the last meeting before the <b>Syndicate</b> gets their hands on the Chaos Drive!";
                else if (Role.ChaosDriveMeetingTimerCount == CustomGameOptions.ChaosDriveMeetingCount)
                    message = "The <b>Syndicate</b> now possesses the Chaos Drive!";
                else
                    message = "The <b>Syndicate</b> possesses the Chaos Drive.";

                HUD.Chat.AddChat(CustomPlayer.Local, message);

                yield return new WaitForSeconds(2f);

                if (Objectifier.GetObjectifiers<Overlord>(ObjectifierEnum.Overlord).Any(x => x.IsAlive))
                {
                    if (MeetingCount == CustomGameOptions.OverlordMeetingWinCount - 1)
                        message = "This is the last meeting to find and kill the <b>Overlord</b>. Should you fail, you will all lose!";
                    else if (MeetingCount < CustomGameOptions.OverlordMeetingWinCount - 1)
                        message = "There seems to be an <b>Overlord</b> bent on dominating the mission! Kill them before they are successful!";

                    if (message != "")
                        HUD.Chat.AddChat(CustomPlayer.Local, message);

                    yield return new WaitForSeconds(2f);
                }

                var knighted = new List<byte>();

                foreach (var monarch in Role.GetRoles<Monarch>(RoleEnum.Monarch))
                {
                    foreach (var id in monarch.ToBeKnighted)
                    {
                        monarch.Knighted.Add(id);

                        if (!knighted.Contains(id))
                        {
                            var knight = PlayerById(id);

                            if (!knight.Data.IsDead && !knight.Data.Disconnected)
                            {
                                message = $"{knight.name} was knighted by a <b>Monarch</b>!";
                                HUD.Chat.AddChat(CustomPlayer.Local, message);
                                knighted.Add(id);

                                if (!CustomGameOptions.KnightButton)
                                    knight.RemainingEmergencies = 0;
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
                ReportedBody = null;
                DisconnectHandler.Disconnected.Clear();

                foreach (var layer in PlayerLayer.LocalLayers)
                {
                    layer?.OnBodyReport(Reported);
                    layer?.OnMeetingStart(Meeting);

                    yield return new WaitForSeconds(0.5f);
                }
            }

            private static string GetLocation(Vector2 position)
            {
                var result = "";

                switch (TownOfUsReworked.NormalOptions.MapId)
                {
                    case 0:

                        if (position.x is < 5 and > -6 && position.y > -4.5f)
                            result = "Cafeteria";
                        else if (position.y is < -0.5f and > -5.4f && position.x is > -11 and < -5.4f)
                            result = "Medbay";
                        else if (position.y is < 3 and > -1.2f && position.x is > -19 and < -15)
                            result = "Upper Engine";
                        else if (position.y is < -1.7f and > -8.2f && position.x < -19.4f)
                            result = "Reactor";
                        else if (position.y is < -3 and > -6.9f && position.x is < -12 and > -14.4f)
                            result = "Security";
                        else if (position.y is < 6.2f and > -4.7f && position.x is < 8 and > 4.7f)
                            result = "O2";
                        else if (position.y is < -9.4f and > -13.6f && position.x is > -19 and < -15)
                            result = "Upper Engine";
                        else if (position.y is < -7.7f and > -13.5f && position.x is > -9.9f and < -5.4f)
                            result = "Electrical";
                        else if (position.y is < -8.7f and > -17 && position.x is > -5f and < 0.7f)
                            result = "Storage";
                        else if (position.y is < -6.5f and > -9.9f && position.x is > 2.2f and < 6.8f)
                            result = "Admin";
                        else if (position.y < -13.8f && position.x is > 1.7f and < 6.4f)
                            result = "Communications";
                        else if (position.y < -10 && position.x > 6.8f)
                            result = "Shields";
                        else if (position.y > -0.8f && position.x > 7.1f)
                            result = "Weapons";
                        else if (position.x > 15)
                            result = "Navigation";
                        else
                            result = "Hallway";

                        break;

                    case 1:

                        if ((position.y is > -1 and < 6 && position.x is > 21.3f and < 29) || (position.x is > 18.1f and < 21.1f && position.y is < 1.3f and > -1))
                            result = "Cafeteria";
                        else if ((position.y is < 5.4f and > 0.6f && position.x is > 9 and < 11) || (position.x is < 8.5f and > 3.9f && position.y is > 0.5f and < 2.3f))
                            result = "Locker";
                        else if (position.y is < -0.5f and > -5.4f && position.x is > -11 and < -5.4f)
                            result = "Medbay";
                        else if (position.y is < 5.6f and > 2.8f && position.x is > 13.8f and < 17)
                            result = "Communications";
                        else if (position.y is < 10 and > 3.3f && position.x is > 5 and < 7.1f)
                            result = "Decontamination";
                        else if (position.y is < 21.2f and > 17.3f && position.x is > 19.2f and < 22.7f)
                            result = "Admin";
                        else if (position.y is < 21.2f and > 17.3f && position.x is > 13 and < 16.4f)
                            result = "Office";
                        else if (position.y is < 5.2f and > 2 && position.x is > 18.1f and < 20.9f)
                            result = "Storage";
                        else if (position.y > 10 && position.x is > 0.2f and < 5)
                            result = "Reactor";
                        else if (position.y > 10 && position.x is > 7.5f and < 12)
                            result = "Laboratory";
                        else if (position.y < -1 && position.x > 18)
                            result = "Balcony";
                        else if (position.y > 21.5f)
                            result = "Greenhouse";
                        else if (position.x < -2.7f)
                            result = "Launchpad";
                        else
                            result = "Hallway";

                        break;

                    case 2:

                        if ((position.y is > -12.5f and < -10.6f && position.x is > 18.6f and < 22.5f) || (position.x is > -11.1f and < -10.6f && position.y is < 18.6f and > 17.4f))
                            result = "Storage";
                        else if ((position.y is < -15.6f and > -22.7f && position.x is > 0.5f and < 4.1f) || (position.x is < 6.7f and > 4.1f && position.y is < -18.6f and > -21.4f))
                            result = "O2";
                        else if (position.y is < -15.5f and > -18.2f && position.x is > 10.3f and < 13)
                            result = "Communications";
                        else if (position.y > -20.3f && position.x is > 16.1f and < 28.7f)
                            result = "Office";
                        else if (position.y < -20.3f && position.x is > 19.8f and < 25.2f)
                            result = "Admin";
                        else if (position.y is < -10.7f and > -12.6f && position.x < 4.2f)
                            result = "Security";
                        else if (position.y is < -8.8f and > -12.6f && position.x < 11.2f)
                            result = "Electrical";
                        else if (position.y < -22.7 && position.x < 4.2)
                            result = "Boiler Room";
                        else if (position.y < -18.7f && position.x > 33.7f)
                            result = "Specimen";
                        else if (position.y > -10.5f && position.x is > 24.7f)
                            result = "Laboratory";
                        else if (position.y > -6)
                            result = "Dropship";
                        else
                            result = "Hallway or Outside";

                        break;

                    case 5 or 6:
                        result = "Somewhere";
                        break;
                }

                return result;
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

                if (CustomPlayer.Local.Is(ModifierEnum.Astral) && !CustomPlayer.Local.inMovingPlat && !CustomPlayer.Local.onLadder)
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
                    !PlayerLayer.GetLayers<Assassin>(LayerEnum.Assassin).Any(x => x.Phone != null) && !Role.GetRoles<Guesser>(RoleEnum.Guesser).Any(x => x.Phone != null));

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
                LogSomething($"Exiled PlayerName = {exiledString}");
                LogSomething($"Was a tie = {tie}");
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

                if ((playerControl.Is(LayerEnum.Assassin, PlayerLayerEnum.Ability) || playerControl.Is(RoleEnum.Guesser)) && playerInfo.IsDead)
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

                        array[i] = new MeetingHud.VoterState
                        {
                            VoterId = playerVoteArea.TargetPlayerId,
                            VotedForId = playerVoteArea.VotedFor
                        };
                    }

                    __instance.RpcVotingComplete(array, exiled, tie);
                    Ability.GetAbilities<Politician>(AbilityEnum.Politician).ForEach(x => CallRpc(CustomRPC.Action, ActionsRPC.SetExtraVotes, x, x.ExtraVotes.ToArray()));
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
                __instance.TitleText.text = UObject.FindObjectOfType<TranslationController>().GetString(StringNames.MeetingVotingResults, Array.Empty<Il2CppSystem.Object>());
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

                foreach (var politician in Ability.GetAbilities<Politician>(AbilityEnum.Politician))
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

                if (CustomPlayer.Local.Is(AbilityEnum.Insider))
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

            if (CustomGameOptions.Whispers)
                name = $"[{player.TargetPlayerId}] " + name;

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

            if (CachedFirstDead != null && player.TargetPlayerId == CachedFirstDead.PlayerId && ((player.TargetPlayerId == CustomPlayer.Local.PlayerId &&
                (int)CustomGameOptions.WhoSeesFirstKillShield == 1) || CustomGameOptions.WhoSeesFirstKillShield == 0))
            {
                name += " <color=#C2185BFF>Γ</color>";
            }

            if (player.Is(RoleEnum.Mayor) && !DeadSeeEverything && CustomPlayer.Local.PlayerId != player.TargetPlayerId)
            {
                var mayor = info[0] as Mayor;

                if (mayor.Revealed)
                {
                    roleRevealed = true;
                    name += $"\n{mayor.Name}";
                    color = mayor.Color;

                    if (CustomPlayer.Local.Is(RoleEnum.Consigliere))
                    {
                        var consigliere = localinfo[0] as Consigliere;

                        if (consigliere.Investigated.Contains(player.TargetPlayerId))
                            consigliere.Investigated.Remove(player.TargetPlayerId);
                    }
                    else if (CustomPlayer.Local.Is(RoleEnum.PromotedGodfather))
                    {
                        var godfather = localinfo[0] as PromotedGodfather;

                        if (godfather.Investigated.Contains(player.TargetPlayerId))
                            godfather.Investigated.Remove(player.TargetPlayerId);
                    }
                    else if (CustomPlayer.Local.Is(RoleEnum.Inspector))
                    {
                        var inspector = localinfo[0] as Inspector;

                        if (inspector.Inspected.Contains(player.TargetPlayerId))
                            inspector.Inspected.Remove(player.TargetPlayerId);
                    }
                    else if (CustomPlayer.Local.Is(RoleEnum.Retributionist))
                    {
                        var retributionist = localinfo[0] as Retributionist;

                        if (retributionist.Inspected.Contains(player.TargetPlayerId))
                            retributionist.Inspected.Remove(player.TargetPlayerId);
                    }
                }
            }
            else if (player.Is(RoleEnum.Dictator) && !DeadSeeEverything && CustomPlayer.Local.PlayerId != player.TargetPlayerId)
            {
                var dict = info[0] as Dictator;

                if (dict.Revealed)
                {
                    roleRevealed = true;
                    name += $"\n{dict.Name}";
                    color = dict.Color;

                    if (CustomPlayer.Local.Is(RoleEnum.Consigliere))
                    {
                        var consigliere = localinfo[0] as Consigliere;

                        if (consigliere.Investigated.Contains(player.TargetPlayerId))
                            consigliere.Investigated.Remove(player.TargetPlayerId);
                    }
                    else if (CustomPlayer.Local.Is(RoleEnum.PromotedGodfather))
                    {
                        var godfather = localinfo[0] as PromotedGodfather;

                        if (godfather.Investigated.Contains(player.TargetPlayerId))
                            godfather.Investigated.Remove(player.TargetPlayerId);
                    }
                    else if (CustomPlayer.Local.Is(RoleEnum.Inspector))
                    {
                        var inspector = localinfo[0] as Inspector;

                        if (inspector.Inspected.Contains(player.TargetPlayerId))
                            inspector.Inspected.Remove(player.TargetPlayerId);
                    }
                    else if (CustomPlayer.Local.Is(RoleEnum.Retributionist))
                    {
                        var retributionist = localinfo[0] as Retributionist;

                        if (retributionist.Inspected.Contains(player.TargetPlayerId))
                            retributionist.Inspected.Remove(player.TargetPlayerId);
                    }
                }
            }

            if (CustomPlayer.Local.Is(RoleEnum.Coroner) && !DeadSeeEverything)
            {
                var coroner = localinfo[0] as Coroner;

                if (coroner.Reported.Contains(player.TargetPlayerId))
                {
                    var role = info[0] as Role;
                    color = role.Color;
                    name += $"\n{role}";
                    roleRevealed = true;
                }
            }
            else if (CustomPlayer.Local.Is(RoleEnum.Consigliere) && !DeadSeeEverything)
            {
                var consigliere = localinfo[0] as Consigliere;

                if (consigliere.Investigated.Contains(player.TargetPlayerId))
                {
                    var role = info[0] as Role;
                    roleRevealed = true;

                    if (CustomGameOptions.ConsigInfo == ConsigInfo.Role)
                    {
                        color = role.Color;
                        name += $"\n{role}";
                    }
                    else if (CustomGameOptions.ConsigInfo == ConsigInfo.Faction)
                    {
                        color = role.FactionColor;
                        name += $"\n{role.FactionName}";
                    }
                }
            }
            else if (CustomPlayer.Local.Is(RoleEnum.PromotedGodfather) && !DeadSeeEverything)
            {
                var godfather = localinfo[0] as PromotedGodfather;

                if (godfather.IsConsig && godfather.Investigated.Contains(player.TargetPlayerId))
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
            else if (CustomPlayer.Local.Is(RoleEnum.Medic))
            {
                var medic = localinfo[0] as Medic;

                if (medic.ShieldedPlayer != null && medic.ShieldedPlayer.PlayerId == player.TargetPlayerId && (int)CustomGameOptions.ShowShielded is 1 or 2)
                    name += " <color=#006600FF>✚</color>";
            }
            else if (CustomPlayer.Local.Is(RoleEnum.Retributionist))
            {
                var ret = localinfo[0] as Retributionist;

                if (ret.IsInsp && ret.Inspected.Contains(player.TargetPlayerId))
                {
                    name += $"\n{player.GetInspResults()}";
                    color = ret.Color;
                    roleRevealed = true;
                }
                else if (ret.IsCor && ret.Reported.Contains(player.TargetPlayerId))
                {
                    var role = info[0] as Role;
                    color = role.Color;
                    name += $"\n{role}";
                    roleRevealed = true;
                }
            }
            else if (CustomPlayer.Local.Is(RoleEnum.Arsonist) && !DeadSeeEverything)
            {
                var arsonist = localinfo[0] as Arsonist;

                if (arsonist.Doused.Contains(player.TargetPlayerId))
                    name += " <color=#EE7600FF>Ξ</color>";
            }
            else if (CustomPlayer.Local.Is(RoleEnum.Plaguebearer) && !DeadSeeEverything)
            {
                var plaguebearer = localinfo[0] as Plaguebearer;

                if (plaguebearer.Infected.Contains(player.TargetPlayerId) && CustomPlayer.Local.PlayerId != player.TargetPlayerId)
                    name += " <color=#CFFE61FF>ρ</color>";
            }
            else if (CustomPlayer.Local.Is(RoleEnum.Cryomaniac) && !DeadSeeEverything)
            {
                var cryomaniac = localinfo[0] as Cryomaniac;

                if (cryomaniac.Doused.Contains(player.TargetPlayerId))
                    name += " <color=#642DEAFF>λ</color>";
            }
            else if (CustomPlayer.Local.Is(RoleEnum.Framer) && !DeadSeeEverything)
            {
                var framer = localinfo[0] as Framer;

                if (framer.Framed.Contains(player.TargetPlayerId))
                    name += " <color=#00FFFFFF>ς</color>";
            }
            else if (CustomPlayer.Local.Is(RoleEnum.Executioner) && !DeadSeeEverything)
            {
                var executioner = localinfo[0] as Executioner;

                if (player.TargetPlayerId == executioner.TargetPlayer.PlayerId)
                {
                    name += " <color=#CCCCCCFF>§</color>";

                    if (CustomGameOptions.ExeKnowsTargetRole)
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
            else if (CustomPlayer.Local.Is(RoleEnum.Guesser) && !DeadSeeEverything)
            {
                var guesser = localinfo[0] as Guesser;

                if (player.TargetPlayerId == guesser.TargetPlayer.PlayerId)
                {
                    color = guesser.Color;
                    name += " <color=#EEE5BEFF>π</color>";
                }
            }
            else if (CustomPlayer.Local.Is(RoleEnum.GuardianAngel) && !DeadSeeEverything)
            {
                var guardianAngel = localinfo[0] as GuardianAngel;

                if (player.TargetPlayerId == guardianAngel.TargetPlayer.PlayerId)
                {
                    name += " <color=#FFFFFFFF>★</color>";

                    if (CustomGameOptions.GAKnowsTargetRole)
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
            else if (CustomPlayer.Local.Is(RoleEnum.Whisperer) && !DeadSeeEverything)
            {
                var whisperer = localinfo[0] as Whisperer;

                if (whisperer.Persuaded.Contains(player.TargetPlayerId))
                {
                    if (CustomGameOptions.FactionSeeRoles)
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
                    foreach (var stats in whisperer.PlayerConversion)
                    {
                        var color2 = (int)(stats.Value / 100f * 256);

                        if (color2 > 0 && player.TargetPlayerId == stats.Key)
                            color = new(255, 255, color2, 255);
                    }
                }
            }
            else if (CustomPlayer.Local.Is(RoleEnum.Dracula) && !DeadSeeEverything)
            {
                var dracula = localinfo[0] as Dracula;

                if (dracula.Converted.Contains(player.TargetPlayerId))
                {
                    if (CustomGameOptions.FactionSeeRoles)
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
            else if (CustomPlayer.Local.Is(RoleEnum.Jackal) && !DeadSeeEverything)
            {
                var jackal = localinfo[0] as Jackal;

                if (jackal.Recruited.Contains(player.TargetPlayerId))
                {
                    if (CustomGameOptions.FactionSeeRoles)
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
            else if (CustomPlayer.Local.Is(RoleEnum.Necromancer) && !DeadSeeEverything)
            {
                var necromancer = localinfo[0] as Necromancer;

                if (necromancer.Resurrected.Contains(player.TargetPlayerId))
                {
                    if (CustomGameOptions.FactionSeeRoles)
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
            else if (CustomPlayer.Local.Is(RoleAlignment.NeutralKill) && !DeadSeeEverything && CustomGameOptions.NKsKnow)
            {
                if ((player.GetRole() == CustomPlayer.Local.GetRole() && CustomGameOptions.NoSolo == NoSolo.SameNKs) || (player.GetAlignment() == CustomPlayer.Local.GetAlignment() &&
                    CustomGameOptions.NoSolo == NoSolo.AllNKs))
                {
                    var role = info[0] as Role;
                    color = role.Color;
                    name += $"\n{role}";
                    roleRevealed = true;
                }
            }
            else if (CustomPlayer.Local.Is(RoleEnum.Inspector))
            {
                var inspector = localinfo[0] as Inspector;

                if (inspector.Inspected.Contains(player.TargetPlayerId))
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
                    if (CustomGameOptions.FactionSeeRoles)
                    {
                        var role = info[0] as Role;
                        color = role.Color;
                        name += $" <color=#7B8968FF>γ</color>\n{role}";
                        roleRevealed = true;

                        if (CustomPlayer.Local.Is(RoleEnum.Consigliere))
                        {
                            var consigliere = localinfo[0] as Consigliere;

                            if (consigliere.Investigated.Contains(player.TargetPlayerId))
                                consigliere.Investigated.Remove(player.TargetPlayerId);
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
                    if (CustomGameOptions.FactionSeeRoles)
                    {
                        var role = info[0] as Role;
                        color = role.Color;
                        name += $" <color=#575657FF>$</color>\n{role}";
                        roleRevealed = true;

                        if (CustomPlayer.Local.Is(RoleEnum.Consigliere))
                        {
                            var consigliere = localinfo[0] as Consigliere;

                            if (consigliere.Investigated.Contains(player.TargetPlayerId))
                                consigliere.Investigated.Remove(player.TargetPlayerId);
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
                    if (CustomGameOptions.FactionSeeRoles)
                    {
                        var role = info[0] as Role;
                        color = role.Color;
                        name += $" <color=#E6108AFF>Σ</color>\n{role}";
                        roleRevealed = true;

                        if (CustomPlayer.Local.Is(RoleEnum.Consigliere))
                        {
                            var consigliere = localinfo[0] as Consigliere;

                            if (consigliere.Investigated.Contains(player.TargetPlayerId))
                                consigliere.Investigated.Remove(player.TargetPlayerId);
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
                    if (CustomGameOptions.FactionSeeRoles)
                    {
                        var role = info[0] as Role;
                        color = role.Color;
                        name += $" <color=#F995FCFF>Λ</color>\n{role}";
                        roleRevealed = true;

                        if (CustomPlayer.Local.Is(RoleEnum.Consigliere))
                        {
                            var consigliere = localinfo[0] as Consigliere;

                            if (consigliere.Investigated.Contains(player.TargetPlayerId))
                                consigliere.Investigated.Remove(player.TargetPlayerId);
                        }
                    }
                    else
                        color = whisperer.SubFactionColor;
                }
            }

            if (CustomPlayer.Local.Is(ObjectifierEnum.Lovers) && !DeadSeeEverything)
            {
                var lover = localinfo[3] as Objectifier;
                var otherLover = CustomPlayer.Local.GetOtherLover();

                if (otherLover.PlayerId == player.TargetPlayerId)
                {
                    name += $" {lover.ColoredSymbol}";

                    if (CustomGameOptions.LoversRoles)
                    {
                        var role = info[0] as Role;
                        color = role.Color;
                        name += $"\n{role}";
                        roleRevealed = true;

                        if (CustomPlayer.Local.Is(RoleEnum.Consigliere))
                        {
                            var consigliere = localinfo[0] as Consigliere;

                            if (consigliere.Investigated.Contains(player.TargetPlayerId))
                                consigliere.Investigated.Remove(player.TargetPlayerId);
                        }
                        else if (CustomPlayer.Local.Is(RoleEnum.PromotedGodfather))
                        {
                            var godfather = localinfo[0] as PromotedGodfather;

                            if (godfather.Investigated.Contains(player.TargetPlayerId))
                                godfather.Investigated.Remove(player.TargetPlayerId);
                        }
                        else if (CustomPlayer.Local.Is(RoleEnum.Inspector))
                        {
                            var inspector = localinfo[0] as Inspector;

                            if (inspector.Inspected.Contains(player.TargetPlayerId))
                                inspector.Inspected.Remove(player.TargetPlayerId);
                        }
                        else if (CustomPlayer.Local.Is(RoleEnum.Retributionist))
                        {
                            var retributionist = localinfo[0] as Retributionist;

                            if (retributionist.Inspected.Contains(player.TargetPlayerId))
                                retributionist.Inspected.Remove(player.TargetPlayerId);
                        }
                    }
                }
            }
            else if (CustomPlayer.Local.Is(ObjectifierEnum.Rivals) && !DeadSeeEverything)
            {
                var rival = localinfo[3] as Objectifier;
                var otherRival = CustomPlayer.Local.GetOtherRival();

                if (otherRival.PlayerId == player.TargetPlayerId)
                {
                    name += $" {rival.ColoredSymbol}";

                    if (CustomGameOptions.RivalsRoles)
                    {
                        var role = info[0] as Role;
                        color = role.Color;
                        name += $"\n{role}";
                        roleRevealed = true;

                        if (CustomPlayer.Local.Is(RoleEnum.Consigliere))
                        {
                            var consigliere = localinfo[0] as Consigliere;

                            if (consigliere.Investigated.Contains(player.TargetPlayerId))
                                consigliere.Investigated.Remove(player.TargetPlayerId);
                        }
                        else if (CustomPlayer.Local.Is(RoleEnum.PromotedGodfather))
                        {
                            var godfather = localinfo[0] as PromotedGodfather;

                            if (godfather.Investigated.Contains(player.TargetPlayerId))
                                godfather.Investigated.Remove(player.TargetPlayerId);
                        }
                        else if (CustomPlayer.Local.Is(RoleEnum.Inspector))
                        {
                            var inspector = localinfo[0] as Inspector;

                            if (inspector.Inspected.Contains(player.TargetPlayerId))
                                inspector.Inspected.Remove(player.TargetPlayerId);
                        }
                        else if (CustomPlayer.Local.Is(RoleEnum.Retributionist))
                        {
                            var retributionist = localinfo[0] as Retributionist;

                            if (retributionist.Inspected.Contains(player.TargetPlayerId))
                                retributionist.Inspected.Remove(player.TargetPlayerId);
                        }
                    }
                }
            }
            else if (CustomPlayer.Local.Is(ObjectifierEnum.Linked) && !DeadSeeEverything)
            {
                var link = localinfo[3] as Objectifier;
                var otherLink = CustomPlayer.Local.GetOtherLink();

                if (otherLink.PlayerId == player.TargetPlayerId)
                {
                    name += $" {link.ColoredSymbol}";

                    if (CustomGameOptions.LinkedRoles)
                    {
                        var role = info[0] as Role;
                        color = role.Color;
                        name += $"\n{role}";
                        roleRevealed = true;

                        if (CustomPlayer.Local.Is(RoleEnum.Consigliere))
                        {
                            var consigliere = localinfo[0] as Consigliere;

                            if (consigliere.Investigated.Contains(player.TargetPlayerId))
                                consigliere.Investigated.Remove(player.TargetPlayerId);
                        }
                        else if (CustomPlayer.Local.Is(RoleEnum.PromotedGodfather))
                        {
                            var godfather = localinfo[0] as PromotedGodfather;

                            if (godfather.Investigated.Contains(player.TargetPlayerId))
                                godfather.Investigated.Remove(player.TargetPlayerId);
                        }
                        else if (CustomPlayer.Local.Is(RoleEnum.Inspector))
                        {
                            var inspector = localinfo[0] as Inspector;

                            if (inspector.Inspected.Contains(player.TargetPlayerId))
                                inspector.Inspected.Remove(player.TargetPlayerId);
                        }
                        else if (CustomPlayer.Local.Is(RoleEnum.Retributionist))
                        {
                            var retributionist = localinfo[0] as Retributionist;

                            if (retributionist.Inspected.Contains(player.TargetPlayerId))
                                retributionist.Inspected.Remove(player.TargetPlayerId);
                        }
                    }
                }
            }
            else if (CustomPlayer.Local.Is(ObjectifierEnum.Mafia) && !DeadSeeEverything)
            {
                var mafia = localinfo[3] as Mafia;

                if (player.Is(ObjectifierEnum.Mafia) && player.TargetPlayerId != CustomPlayer.Local.PlayerId)
                {
                    name += $" {mafia.ColoredSymbol}";

                    if (CustomGameOptions.MafiaRoles)
                    {
                        var role = info[0] as Role;
                        color = role.Color;
                        name += $"\n{role}";
                        roleRevealed = true;

                        if (CustomPlayer.Local.Is(RoleEnum.Consigliere))
                        {
                            var consigliere = localinfo[0] as Consigliere;

                            if (consigliere.Investigated.Contains(player.TargetPlayerId))
                                consigliere.Investigated.Remove(player.TargetPlayerId);
                        }
                        else if (CustomPlayer.Local.Is(RoleEnum.PromotedGodfather))
                        {
                            var godfather = localinfo[0] as PromotedGodfather;

                            if (godfather.Investigated.Contains(player.TargetPlayerId))
                                godfather.Investigated.Remove(player.TargetPlayerId);
                        }
                        else if (CustomPlayer.Local.Is(RoleEnum.Inspector))
                        {
                            var inspector = localinfo[0] as Inspector;

                            if (inspector.Inspected.Contains(player.TargetPlayerId))
                                inspector.Inspected.Remove(player.TargetPlayerId);
                        }
                        else if (CustomPlayer.Local.Is(RoleEnum.Retributionist))
                        {
                            var retributionist = localinfo[0] as Retributionist;

                            if (retributionist.Inspected.Contains(player.TargetPlayerId))
                                retributionist.Inspected.Remove(player.TargetPlayerId);
                        }
                    }
                }
            }

            if (CustomPlayer.Local.Is(AbilityEnum.Snitch) && CustomGameOptions.SnitchSeestargetsInMeeting && !DeadSeeEverything && player.TargetPlayerId != CustomPlayer.Local.PlayerId)
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
                        if (!(player.Is(ObjectifierEnum.Traitor) && CustomGameOptions.SnitchSeesTraitor) && !(player.Is(ObjectifierEnum.Fanatic) && CustomGameOptions.SnitchSeesFanatic))
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

            if (player.Is(AbilityEnum.Snitch))
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

            if (CustomPlayer.Local.GetFaction() == player.GetFaction() && player.TargetPlayerId != CustomPlayer.Local.PlayerId && (player.GetFaction() == Faction.Intruder ||
                player.GetFaction() == Faction.Syndicate) && !DeadSeeEverything)
            {
                var role = info[0] as Role;

                if (CustomGameOptions.FactionSeeRoles)
                {
                    color = role.Color;
                    name += $"\n{role}";
                    roleRevealed = true;

                    if (CustomPlayer.Local.Is(RoleEnum.Consigliere))
                    {
                        var consigliere = localinfo[0] as Consigliere;

                        if (consigliere.Investigated.Contains(player.TargetPlayerId))
                            consigliere.Investigated.Remove(player.TargetPlayerId);
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

            if (CustomPlayer.Local.Is(Faction.Syndicate) && player == Role.DriveHolder)
                name += " <color=#008000FF>Δ</color>";

            if (Role.GetRoles<Revealer>(RoleEnum.Revealer).Any(x => x.CompletedTasks) && CustomPlayer.Local.Is(Faction.Crew))
            {
                var role = info[0] as Role;

                if (CustomGameOptions.SnitchSeesRoles)
                {
                    if (player.Is(Faction.Syndicate) || player.Is(Faction.Intruder) || (player.Is(Faction.Neutral) && CustomGameOptions.RevealerRevealsNeutrals) ||
                        (player.Is(Faction.Crew) && CustomGameOptions.RevealerRevealsCrew))
                    {
                        color = role.Color;
                        name += $"\n{role}";
                        roleRevealed = true;
                    }
                }
                else if (player.Is(Faction.Syndicate) || player.Is(Faction.Intruder) || (player.Is(Faction.Neutral) && CustomGameOptions.RevealerRevealsNeutrals) ||
                    (player.Is(Faction.Crew) && CustomGameOptions.RevealerRevealsCrew))
                {
                    if (!(player.Is(ObjectifierEnum.Traitor) && CustomGameOptions.RevealerRevealsTraitor) && !(player.Is(ObjectifierEnum.Fanatic) &&
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

                if (CustomPlayer.Local.Is(RoleEnum.Consigliere))
                {
                    var consigliere = localinfo[0] as Consigliere;
                    consigliere.Investigated.Clear();
                }

                if (CustomPlayer.Local.Is(RoleEnum.Inspector))
                {
                    var inspector = localinfo[0] as Inspector;
                    inspector.Inspected.Clear();
                }
            }

            if (DeadSeeEverything || player.TargetPlayerId == CustomPlayer.Local.PlayerId)
            {
                if (info[3])
                {
                    var objectifier = info[3] as Objectifier;

                    if (objectifier.ObjectifierType != ObjectifierEnum.None && !objectifier.Hidden)
                        name += $" {objectifier.ColoredSymbol}";
                }

                var role = info[0] as Role;
                color = role.Color;
                name += $"\n{role}";
                roleRevealed = true;
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
            foreach (var role in Ability.GetAbilities<Swapper>(AbilityEnum.Swapper))
            {
                if (role.IsDead || role.Disconnected || role.Swap1 == null || role.Swap2 == null)
                    continue;

                var swapPlayer1 = PlayerByVoteArea(role.Swap1);
                var swapPlayer2 = PlayerByVoteArea(role.Swap2);

                if (swapPlayer1 == null || swapPlayer2 == null || swapPlayer1.Data.IsDead || swapPlayer1.Data.Disconnected || swapPlayer2.Data.IsDead || swapPlayer2.Data.Disconnected)
                    continue;

                var pool1 = role.Swap1.PlayerIcon.transform;
                var name1 = role.Swap1.NameText.transform;
                var background1 = role.Swap1.Background.transform;
                var mask1 = role.Swap1.MaskArea.transform;
                var whiteBackground1 = role.Swap1.PlayerButton.transform;
                var level1 = role.Swap1.LevelNumberText.GetComponentInParent<SpriteRenderer>().transform;
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
                var level2 = role.Swap1.LevelNumberText.GetComponentInParent<SpriteRenderer>().transform;
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

                var duration = 1f / Ability.GetAbilities(AbilityEnum.Swapper).Count;

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
}