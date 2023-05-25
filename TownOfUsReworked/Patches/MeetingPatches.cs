namespace TownOfUsReworked.Patches
{
    [HarmonyPatch]
    public static class MeetingPatches
    {
        private static GameData.PlayerInfo VoteTarget;
        public static int MeetingCount;
        private static GameData.PlayerInfo Reported = null;
        public static bool GivingAnnouncements = false;
        private static DeadBody ReportedBody = null;

        [HarmonyPatch(typeof(PlayerVoteArea), nameof(PlayerVoteArea.SetCosmetics))]
        public static class PlayerStates
        {
            public static void Postfix(PlayerVoteArea __instance)
            {
                if (CustomGameOptions.MeetingColourblind && DoUndo.IsCamoed)
                {
                    __instance.Background.sprite = HatManager.Instance.GetNamePlateById("nameplate_NoPlate").viewData.viewData.Image;
                    __instance.LevelNumberText.GetComponentInParent<SpriteRenderer>().enabled = false;
                    __instance.LevelNumberText.GetComponentInParent<SpriteRenderer>().gameObject.SetActive(false);
                    __instance.NameText.color = Palette.White;
                }
                else
                {
                    if (CustomGameOptions.WhiteNameplates)
                        __instance.Background.sprite = HatManager.Instance.GetNamePlateById("nameplate_NoPlate").viewData.viewData.Image;

                    if (CustomGameOptions.DisableLevels)
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
                ReportedBody = target == null ? null : Utils.BodyById(target.PlayerId);
            }
        }

        [HarmonyPatch(typeof(PlayerVoteArea), nameof(PlayerVoteArea.PreviewNameplate))]
        public static class PlayerPreviews
        {
            public static void Postfix(PlayerVoteArea __instance)
            {
                if (CustomGameOptions.MeetingColourblind && DoUndo.IsCamoed)
                {
                    __instance.Background.sprite = HatManager.Instance.GetNamePlateById("nameplate_NoPlate").viewData.viewData.Image;
                    __instance.LevelNumberText.GetComponentInParent<SpriteRenderer>().enabled = false;
                    __instance.LevelNumberText.GetComponentInParent<SpriteRenderer>().gameObject.SetActive(false);
                    __instance.NameText.color = Palette.White;
                }
                else
                {
                    if (CustomGameOptions.WhiteNameplates)
                        __instance.Background.sprite = HatManager.Instance.GetNamePlateById("nameplate_NoPlate").viewData.viewData.Image;

                    if (CustomGameOptions.DisableLevels)
                    {
                        __instance.LevelNumberText.GetComponentInParent<SpriteRenderer>().enabled = false;
                        __instance.LevelNumberText.GetComponentInParent<SpriteRenderer>().gameObject.SetActive(false);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
        [HarmonyPriority(Priority.First)]
        public static class MeetingHUD_Start
        {
            public static void Postfix(MeetingHud __instance)
            {
                __instance.gameObject.AddComponent<MeetingHudPagingBehaviour>().meetingHud = __instance;
                MeetingCount++;
                Coroutines.Start(Announcements());

                foreach (var player in PlayerControl.AllPlayerControls)
                    player?.MyPhysics?.ResetAnimState();

                if (Role.ChaosDriveMeetingTimerCount < CustomGameOptions.ChaosDriveMeetingCount)
                    Role.ChaosDriveMeetingTimerCount++;

                if (Role.ChaosDriveMeetingTimerCount == CustomGameOptions.ChaosDriveMeetingCount && !Role.SyndicateHasChaosDrive)
                {
                    Role.SyndicateHasChaosDrive = true;
                    RoleGen.AssignChaosDrive();
                }

                foreach (var layer in PlayerLayer.LocalLayers)
                    layer?.OnMeetingStart(__instance);
            }

            private static IEnumerator Announcements()
            {
                foreach (var layer in PlayerLayer.LocalLayers)
                    layer?.OnBodyReport(Reported);

                yield return new WaitForSeconds(5f);

                GivingAnnouncements = true;

                if (CustomGameOptions.GameAnnouncements)
                {
                    PlayerControl check = null;

                    if (Reported != null)
                    {
                        var player = Reported.Object;
                        check = player;
                        var report = $"{player.name} was found dead last round.";
                        HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, report);

                        yield return new WaitForSeconds(2f);

                        if (CustomGameOptions.LocationReports)
                            report = $"Their body was found in {GetLocation(Utils.BodyById(player.PlayerId).TruePosition)}.";
                        else
                            report = "It is unknown where they died.";

                        HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, report);

                        yield return new WaitForSeconds(2f);

                        var killer = Utils.PlayerById(Utils.KilledPlayers.Find(x => x.PlayerId == player.PlayerId).KillerId);
                        var flag = killer.Is(RoleEnum.Altruist) || killer.Is(RoleEnum.Arsonist) || killer.Is(RoleEnum.Amnesiac) || killer.Is(RoleEnum.Executioner) ||
                            killer.Is(RoleEnum.Engineer) || killer.Is(RoleEnum.Escort) || killer.Is(RoleEnum.Impostor) || killer.Is(RoleEnum.Inspector) || killer.Is(RoleEnum.Operative);
                        var a_an = flag ? "an" : "a";
                        var flag1 = killer.Is(Faction.Intruder) || killer.Is(Faction.Neutral);
                        var s = flag1 ? "s" : "";
                        var killerRole = Role.GetRole(killer);

                        if (CustomGameOptions.KillerReports == RoleFactionReports.Role)
                            report = $"They were killed by {a_an} {killerRole.Name}.";
                        else if (CustomGameOptions.KillerReports == RoleFactionReports.Faction)
                            report = $"They were killed by a member of the {killerRole.FactionName}{s}.";
                        else
                            report = "They were killed by an unknown assailant.";

                        HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, report);

                        yield return new WaitForSeconds(2f);

                        var flag2 = player.Is(RoleEnum.Altruist) || player.Is(RoleEnum.Arsonist) || player.Is(RoleEnum.Amnesiac) || player.Is(RoleEnum.Executioner) ||
                            player.Is(RoleEnum.Engineer) || player.Is(RoleEnum.Escort) || player.Is(RoleEnum.Impostor) || player.Is(RoleEnum.Inspector) || player.Is(RoleEnum.Operative);
                        var a_an2 = flag2 ? "an" : "a";
                        var role = Role.GetRole(player);

                        if (CustomGameOptions.RoleFactionReports == RoleFactionReports.Role)
                            report = $"They were {a_an2} {role.Name}.";
                        else if (CustomGameOptions.RoleFactionReports == RoleFactionReports.Faction)
                            report = $"They were from the {role.FactionName} faction.";
                        else
                            report = "It is unknown what they were.";

                        HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, report);
                    }
                    else
                        HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, "A meeting has been called.");

                    yield return new WaitForSeconds(2f);

                    foreach (var player in Utils.RecentlyKilled)
                    {
                        if (player != check)
                        {
                            var report = $"{player.name} was found dead last round.";
                            HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, report);

                            yield return new WaitForSeconds(2f);

                            report = "It is unknown where they died.";
                            HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, report);

                            yield return new WaitForSeconds(2f);

                            var killer = Utils.PlayerById(Utils.KilledPlayers.Find(x => x.PlayerId == player.PlayerId).KillerId);
                            var killerRole = Role.GetRole(killer);
                            var flag = killer.Is(RoleEnum.Altruist) || killer.Is(RoleEnum.Arsonist) || killer.Is(RoleEnum.Amnesiac) || killer.Is(RoleEnum.Executioner) ||
                                killer.Is(RoleEnum.Engineer) || killer.Is(RoleEnum.Escort) || killer.Is(RoleEnum.Impostor) || killer.Is(RoleEnum.Inspector) ||
                                killer.Is(RoleEnum.Operative);
                            var a_an = flag ? "an" : "a";

                            if (Role.Cleaned.Contains(player))
                                report = "They were killed by an unknown assailant.";
                            else if (CustomGameOptions.KillerReports == RoleFactionReports.Role)
                                report = $"They were killed by {a_an} {killerRole.Name}.";
                            else if (CustomGameOptions.KillerReports == RoleFactionReports.Faction)
                                report = $"They were killed by a member of the {killerRole.FactionName}.";
                            else
                                report = "They were killed by an unknown assailant.";

                            HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, report);

                            yield return new WaitForSeconds(2f);

                            var role = Role.GetRole(player);
                            var flag2 = player.Is(RoleEnum.Altruist) || player.Is(RoleEnum.Arsonist) || player.Is(RoleEnum.Amnesiac) || player.Is(RoleEnum.Executioner) ||
                                player.Is(RoleEnum.Engineer) || player.Is(RoleEnum.Escort) || player.Is(RoleEnum.Impostor) || player.Is(RoleEnum.Inspector) ||
                                player.Is(RoleEnum.Operative);
                            var a_an2 = flag2 ? "an" : "a";

                            if (Role.Cleaned.Contains(player))
                                report = $"We could not determine what {player.name} was.";
                            else if (CustomGameOptions.RoleFactionReports == RoleFactionReports.Role)
                                report = $"They were {a_an2} {role.Name}.";
                            else if (CustomGameOptions.RoleFactionReports == RoleFactionReports.Faction)
                                report = $"They were from the {role.FactionName} faction.";
                            else
                                report = $"We could not determine what {player.name} was.";

                            HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, report);

                            yield return new WaitForSeconds(2f);
                        }
                    }

                    foreach (var player in DisconnectHandler.Disconnected)
                    {
                        if (player != check)
                        {
                            HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, $"{player.name} escaped the ship last round.");

                            yield return new WaitForSeconds(2f);
                        }
                    }
                }

                var message = "";

                if (Role.ChaosDriveMeetingTimerCount < CustomGameOptions.ChaosDriveMeetingCount - 1)
                    message = $"{CustomGameOptions.ChaosDriveMeetingCount - Role.ChaosDriveMeetingTimerCount} meetings remain till the Syndicate gets their hands on the Chaos Drive!";
                else if (Role.ChaosDriveMeetingTimerCount == CustomGameOptions.ChaosDriveMeetingCount - 1)
                    message = "This is the last meeting before the Syndicate gets their hands on the Chaos Drive!";
                else if (Role.ChaosDriveMeetingTimerCount == CustomGameOptions.ChaosDriveMeetingCount)
                    message = "The Syndicate now possesses the Chaos Drive!";
                else
                    message = "The Syndicate possesses the Chaos Drive.";

                HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, message);

                yield return new WaitForSeconds(2f);

                if (Objectifier.GetObjectifiers<Overlord>(ObjectifierEnum.Overlord).Any(x => x.IsAlive))
                {
                    if (MeetingCount == CustomGameOptions.OverlordMeetingWinCount - 1)
                        message = "This is the last meeting to find and kill the Overlord. Should you fail, you will all lose!";
                    else if (MeetingCount < CustomGameOptions.OverlordMeetingWinCount - 1)
                        message = "There seems to be an Overlord bent on dominating the mission! Kill them before they are successful!";

                    if (message != "")
                        HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, message);

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
                            message = $"{Utils.PlayerById(id).name} was knighted by a Monarch!";
                            HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, message);
                            knighted.Add(id);
                        }

                        yield return new WaitForSeconds(2f);
                    }

                    monarch.ToBeKnighted.Clear();
                }

                if (!CustomGameOptions.KnightButton)
                {
                    foreach (var id in knighted)
                        Utils.PlayerById(id).RemainingEmergencies = 0;
                }

                Utils.RecentlyKilled.Clear();
                Role.Cleaned.Clear();
                GivingAnnouncements = false;
                Reported = null;
                ReportedBody = null;
                DisconnectHandler.Disconnected.Clear();
            }

            private static string GetLocation(Vector2 position)
            {
                var result = "";

                switch (TownOfUsReworked.VanillaOptions.MapId)
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

                    case 5:
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
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.MeetingStart, SendOption.Reliable);
                AmongUsClient.Instance.FinishRpcImmediately(writer);

                foreach (var body in Utils.AllBodies)
                    body.gameObject.Destroy();

                foreach (var player in PlayerControl.AllPlayerControls)
                    player.MyPhysics.ResetAnimState();

                foreach (var layer in PlayerLayer.LocalLayers)
                    layer?.OnMeetingEnd(__instance);
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.StartMeeting))]
        public static class StartMeetingPatch
        {
            public static void Prefix([HarmonyArgument(0)] GameData.PlayerInfo meetingTarget) => VoteTarget = meetingTarget;
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
        [HarmonyPriority(Priority.First)]
        public static class MeetingHudUpdatePatch
        {
            public static void Postfix(MeetingHud __instance)
            {
                //Deactivate skip Button if skipping on emergency meetings is disabled
                __instance.SkipVoteButton.gameObject.SetActive(!((VoteTarget == null && CustomGameOptions.SkipButtonDisable == DisableSkipButtonMeetings.Emergency) ||
                    (CustomGameOptions.SkipButtonDisable == DisableSkipButtonMeetings.Always)));

                if (CustomGameOptions.MeetingColourblind && DoUndo.IsCamoed)
                {
                    foreach (var state in __instance.playerStates)
                    {
                        state.PlayerIcon.SetBodyColor(6);
                        state.PlayerIcon.SetHat("hat_noHat", 0);
                        state.PlayerIcon.SetSkin("None", 0);
                        state.PlayerIcon.SetVisor("visor_noVisor", 0);
                    }
                }

                __instance.playerStates.ToList().ForEach(x => x.SetName());

                foreach (var layer in PlayerLayer.LocalLayers)
                    layer.UpdateMeeting(__instance);

                if (!PlayerControl.LocalPlayer.Is(AbilityEnum.Politician) || PlayerControl.LocalPlayer.Data.IsDead || __instance.TimerText.text.Contains("Can Vote"))
                    return;

                __instance.TimerText.text = $"Can Vote: {Ability.GetAbility<Politician>(PlayerControl.LocalPlayer).VoteBank} time(s) | {__instance.TimerText.text}";
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.VotingComplete))]
        public static class VotingComplete
        {
            public static void Postfix(MeetingHud __instance, [HarmonyArgument(1)] GameData.PlayerInfo exiled, [HarmonyArgument(2)] bool tie)
            {
                var exiledString = exiled == null ? "null" : exiled.PlayerName;
                Utils.LogSomething($"Exiled PlayerName = {exiledString}");
                Utils.LogSomething($"Was a tie = {tie}");

                foreach (var layer in PlayerLayer.LocalLayers)
                    layer.VoteComplete(__instance);

                Coroutines.Start(PerformSwaps());

                foreach (var layer in PlayerLayer.LocalLayers)
                    layer?.OnMeetingEnd(__instance);
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Select))]
        public static class MeetingHudSelect
        {
            public static void Postfix(MeetingHud __instance, int __0)
            {
                foreach (var layer in PlayerLayer.LocalLayers)
                    layer.SelectVote(__instance, __0);
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.ClearVote))]
        public static class MeetingHudClearVote
        {
            public static void Postfix(MeetingHud __instance)
            {
                foreach (var layer in PlayerLayer.LocalLayers)
                    layer.ClearVote(__instance);
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

                if ((playerControl.Is(AbilityEnum.Assassin) || playerControl.Is(RoleEnum.Guesser)) && playerInfo.IsDead)
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

                    foreach (var role in Ability.GetAbilities<Politician>(AbilityEnum.Politician))
                    {
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                        writer.Write((byte)ActionsRPC.SetExtraVotes);
                        writer.Write(role.PlayerId);
                        writer.WriteBytesAndSize(role.ExtraVotes.ToArray());
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                    }
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
                    TownOfUsReworked.VanillaOptions.AnonymousVotes = CustomGameOptions.PoliticianAnonymous;

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

                    TownOfUsReworked.VanillaOptions.AnonymousVotes = CustomGameOptions.AnonymousVoting;
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
                var deadFlag = CustomGameOptions.DeadSeeEverything && PlayerControl.LocalPlayer.Data.IsDead;

                if (PlayerControl.LocalPlayer.Is(AbilityEnum.Insider))
                    insiderFlag = Role.LocalRole.TasksDone;

                if (TownOfUsReworked.VanillaOptions.AnonymousVotes && !(deadFlag || insiderFlag))
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
            if (DoUndo.IsCamoed && CustomGameOptions.MeetingColourblind)
                return ("", Color.clear);

            var color = Color.white;
            var name = UpdateNames.PlayerNames.FirstOrDefault(x => x.Key == player.TargetPlayerId).Value;
            var info = player.AllPlayerInfo();
            var localinfo = PlayerControl.LocalPlayer.AllPlayerInfo();
            var roleRevealed = false;

            if (CustomGameOptions.Whispers && !(DoUndo.IsCamoed && CustomGameOptions.MeetingColourblind))
                name = $"[{player.TargetPlayerId}] " + name;

            if (info[0] == null || localinfo[0] == null)
                return (name, color);

            if (player.CanDoTasks() && (PlayerControl.LocalPlayer.PlayerId == player.TargetPlayerId || ConstantVariables.DeadSeeEverything))
            {
                var role = info[0] as Role;
                name += $"{((DoUndo.IsCamoed && CustomGameOptions.MeetingColourblind) || PlayerControl.LocalPlayer.Data.IsDead ? name : "")} ({role.TasksCompleted}/{role.TotalTasks})";
                roleRevealed = true;
            }

            if (player.IsKnighted())
                name += " <color=#FF004EFF>κ</color>";

            if (player.IsSpelled())
                name += " <color=#0028F5FF>ø</color>";

            if (player.Is(RoleEnum.Mayor) && !ConstantVariables.DeadSeeEverything && PlayerControl.LocalPlayer.PlayerId != player.TargetPlayerId)
            {
                var mayor = info[0] as Mayor;

                if (mayor.Revealed)
                {
                    roleRevealed = true;
                    name += $"\n{mayor.Name}";
                    color = mayor.Color;

                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Consigliere))
                    {
                        var consigliere = localinfo[0] as Consigliere;

                        if (consigliere.Investigated.Contains(player.TargetPlayerId))
                            consigliere.Investigated.Remove(player.TargetPlayerId);
                    }
                    else if (PlayerControl.LocalPlayer.Is(RoleEnum.Inspector))
                    {
                        var inspector = localinfo[0] as Inspector;

                        if (inspector.Inspected.Contains(player.TargetPlayerId))
                            inspector.Inspected.Remove(player.TargetPlayerId);
                    }
                    else if (PlayerControl.LocalPlayer.Is(RoleEnum.Retributionist))
                    {
                        var retributionist = localinfo[0] as Retributionist;

                        if (retributionist.Inspected.Contains(player.TargetPlayerId))
                            retributionist.Inspected.Remove(player.TargetPlayerId);
                    }
                }
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Coroner) && !ConstantVariables.DeadSeeEverything)
            {
                var coroner = localinfo[0] as Coroner;

                if (coroner.Reported.Contains(player.TargetPlayerId))
                {
                    var role = info[0] as Role;
                    color = role.Color;
                    name += $"\n{role.Name}";
                    roleRevealed = true;
                }
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Consigliere) && !ConstantVariables.DeadSeeEverything)
            {
                var consigliere = localinfo[0] as Consigliere;

                if (consigliere.Investigated.Contains(player.TargetPlayerId))
                {
                    var role = info[0] as Role;
                    roleRevealed = true;

                    if (CustomGameOptions.ConsigInfo == ConsigInfo.Role)
                    {
                        color = role.Color;
                        name += $"\n{role.Name}";
                    }
                    else if (CustomGameOptions.ConsigInfo == ConsigInfo.Faction)
                    {
                        color = role.FactionColor;
                        name += $"\n{role.FactionName}";
                    }
                }
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.PromotedGodfather) && !ConstantVariables.DeadSeeEverything)
            {
                var godfather = localinfo[0] as PromotedGodfather;

                if (godfather.IsConsig && godfather.Investigated.Contains(player.TargetPlayerId))
                {
                    var role = info[0] as Role;
                    roleRevealed = true;

                    if (CustomGameOptions.ConsigInfo == ConsigInfo.Role)
                    {
                        color = role.Color;
                        name += $"\n{role.Name}";

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
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Medic))
            {
                var medic = localinfo[0] as Medic;

                if (medic.ShieldedPlayer != null && medic.ShieldedPlayer.PlayerId == player.TargetPlayerId && (int)CustomGameOptions.ShowShielded is 1 or 2)
                    name += " <color=#006600FF>✚</color>";
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Retributionist))
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
                    name += $"\n{role.Name}";
                    roleRevealed = true;
                }
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Arsonist) && !ConstantVariables.DeadSeeEverything)
            {
                var arsonist = localinfo[0] as Arsonist;

                if (arsonist.Doused.Contains(player.TargetPlayerId))
                    name += " <color=#EE7600FF>Ξ</color>";
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Plaguebearer) && !ConstantVariables.DeadSeeEverything)
            {
                var plaguebearer = localinfo[0] as Plaguebearer;

                if (plaguebearer.Infected.Contains(player.TargetPlayerId) && PlayerControl.LocalPlayer.PlayerId != player.TargetPlayerId)
                    name += " <color=#CFFE61FF>ρ</color>";
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Cryomaniac) && !ConstantVariables.DeadSeeEverything)
            {
                var cryomaniac = localinfo[0] as Cryomaniac;

                if (cryomaniac.Doused.Contains(player.TargetPlayerId))
                    name += " <color=#642DEAFF>λ</color>";
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Framer) && !ConstantVariables.DeadSeeEverything)
            {
                var framer = localinfo[0] as Framer;

                if (framer.Framed.Contains(player.TargetPlayerId))
                    name += " <color=#00FFFFFF>ς</color>";
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Executioner) && !ConstantVariables.DeadSeeEverything)
            {
                var executioner = localinfo[0] as Executioner;

                if (player.TargetPlayerId == executioner.TargetPlayer.PlayerId)
                {
                    name += " <color=#CCCCCCFF>§</color>";

                    if (CustomGameOptions.ExeKnowsTargetRole)
                    {
                        var role = info[0] as Role;
                        color = role.Color;
                        name += $"\n{role.Name}";
                        roleRevealed = true;
                    }
                    else
                        color = executioner.Color;
                }
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Guesser) && !ConstantVariables.DeadSeeEverything)
            {
                var guesser = localinfo[0] as Guesser;

                if (player.TargetPlayerId == guesser.TargetPlayer.PlayerId)
                {
                    color = guesser.Color;
                    name += " <color=#EEE5BEFF>π</color>";
                }
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.GuardianAngel) && !ConstantVariables.DeadSeeEverything)
            {
                var guardianAngel = localinfo[0] as GuardianAngel;

                if (player.TargetPlayerId == guardianAngel.TargetPlayer.PlayerId)
                {
                    name += " <color=#FFFFFFFF>★</color>";

                    if (CustomGameOptions.GAKnowsTargetRole)
                    {
                        var role = info[0] as Role;
                        color = role.Color;
                        name += $"\n{role.Name}";
                        roleRevealed = true;
                    }
                    else
                        color = guardianAngel.Color;
                }
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Whisperer) && !ConstantVariables.DeadSeeEverything)
            {
                var whisperer = localinfo[0] as Whisperer;

                if (whisperer.Persuaded.Contains(player.TargetPlayerId))
                {
                    if (CustomGameOptions.FactionSeeRoles)
                    {
                        var role = info[0] as Role;
                        color = role.Color;
                        name += $" <color=#F995FCFF>Λ</color>\n{role.Name}";
                        roleRevealed = true;
                    }
                    else
                        color = whisperer.SubFactionColor;
                }
                else
                {
                    foreach (var stats in whisperer.PlayerConversion)
                    {
                        var color2 = (int)(stats.Item2 / 100f * 256);

                        if (color2 > 0 && player.TargetPlayerId == stats.Item1)
                            color = new(255, 255, color2, 255);
                    }
                }
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Dracula) && !ConstantVariables.DeadSeeEverything)
            {
                var dracula = localinfo[0] as Dracula;

                if (dracula.Converted.Contains(player.TargetPlayerId))
                {
                    if (CustomGameOptions.FactionSeeRoles)
                    {
                        var role = info[0] as Role;
                        color = role.Color;
                        name += $" <color=#7B8968FF>γ</color>\n{role.Name}";
                        roleRevealed = true;
                    }
                    else
                        color = dracula.SubFactionColor;
                }
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Jackal) && !ConstantVariables.DeadSeeEverything)
            {
                var jackal = localinfo[0] as Jackal;

                if (jackal.Recruited.Contains(player.TargetPlayerId))
                {
                    if (CustomGameOptions.FactionSeeRoles)
                    {
                        var role = info[0] as Role;
                        color = role.Color;
                        name += $" <color=#575657FF>$</color>\n{role.Name}";
                        roleRevealed = true;
                    }
                    else
                        color = jackal.SubFactionColor;
                }
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Necromancer) && !ConstantVariables.DeadSeeEverything)
            {
                var necromancer = localinfo[0] as Necromancer;

                if (necromancer.Resurrected.Contains(player.TargetPlayerId))
                {
                    if (CustomGameOptions.FactionSeeRoles)
                    {
                        var role = info[0] as Role;
                        color = role.Color;
                        name += $" <color=#E6108AFF>Σ</color>\n{role.Name}";
                        roleRevealed = true;
                    }
                    else
                        color = necromancer.SubFactionColor;
                }
            }

            if (PlayerControl.LocalPlayer.IsBitten() && !ConstantVariables.DeadSeeEverything)
            {
                var dracula = PlayerControl.LocalPlayer.GetDracula();

                if (dracula.Converted.Contains(player.TargetPlayerId))
                {
                    if (CustomGameOptions.FactionSeeRoles)
                    {
                        var role = info[0] as Role;
                        color = role.Color;
                        name += $" <color=#7B8968FF>γ</color>\n{role.Name}";
                        roleRevealed = true;

                        if (PlayerControl.LocalPlayer.Is(RoleEnum.Consigliere))
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
            else if (PlayerControl.LocalPlayer.IsRecruit() && !ConstantVariables.DeadSeeEverything)
            {
                var jackal = PlayerControl.LocalPlayer.GetJackal();

                if (jackal.Recruited.Contains(player.TargetPlayerId))
                {
                    if (CustomGameOptions.FactionSeeRoles)
                    {
                        var role = info[0] as Role;
                        color = role.Color;
                        name += $" <color=#575657FF>$</color>\n{role.Name}";
                        roleRevealed = true;

                        if (PlayerControl.LocalPlayer.Is(RoleEnum.Consigliere))
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
            else if (PlayerControl.LocalPlayer.IsResurrected() && !ConstantVariables.DeadSeeEverything)
            {
                var necromancer = PlayerControl.LocalPlayer.GetNecromancer();

                if (necromancer.Resurrected.Contains(player.TargetPlayerId))
                {
                    if (CustomGameOptions.FactionSeeRoles)
                    {
                        var role = info[0] as Role;
                        color = role.Color;
                        name += $" <color=#E6108AFF>Σ</color>\n{role.Name}";
                        roleRevealed = true;

                        if (PlayerControl.LocalPlayer.Is(RoleEnum.Consigliere))
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
            else if (PlayerControl.LocalPlayer.IsPersuaded() && !ConstantVariables.DeadSeeEverything)
            {
                var whisperer = PlayerControl.LocalPlayer.GetWhisperer();

                if (whisperer.Persuaded.Contains(player.TargetPlayerId))
                {
                    if (CustomGameOptions.FactionSeeRoles)
                    {
                        var role = info[0] as Role;
                        color = role.Color;
                        name += $" <color=#F995FCFF>Λ</color>\n{role.Name}";
                        roleRevealed = true;

                        if (PlayerControl.LocalPlayer.Is(RoleEnum.Consigliere))
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

            if (PlayerControl.LocalPlayer.Is(ObjectifierEnum.Lovers) && !ConstantVariables.DeadSeeEverything)
            {
                var lover = localinfo[3] as Objectifier;
                var otherLover = PlayerControl.LocalPlayer.GetOtherLover();

                if (otherLover == player)
                {
                    name += $" {lover.ColoredSymbol}";

                    if (CustomGameOptions.LoversRoles)
                    {
                        var role = info[0] as Role;
                        color = role.Color;
                        name += $"\n{role.Name}";
                        roleRevealed = true;

                        if (PlayerControl.LocalPlayer.Is(RoleEnum.Consigliere))
                        {
                            var consigliere = localinfo[0] as Consigliere;

                            if (consigliere.Investigated.Contains(player.TargetPlayerId))
                                consigliere.Investigated.Remove(player.TargetPlayerId);
                        }
                    }
                }
            }
            else if (PlayerControl.LocalPlayer.Is(ObjectifierEnum.Rivals) && !ConstantVariables.DeadSeeEverything)
            {
                var rival = localinfo[3] as Objectifier;
                var otherRival = PlayerControl.LocalPlayer.GetOtherRival();

                if (otherRival == player)
                {
                    name += $" {rival.ColoredSymbol}";

                    if (CustomGameOptions.RivalsRoles)
                    {
                        var role = info[0] as Role;
                        color = role.Color;
                        name += $"\n{role.Name}";
                        roleRevealed = true;

                        if (PlayerControl.LocalPlayer.Is(RoleEnum.Consigliere))
                        {
                            var consigliere = localinfo[0] as Consigliere;

                            if (consigliere.Investigated.Contains(player.TargetPlayerId))
                                consigliere.Investigated.Remove(player.TargetPlayerId);
                        }
                    }
                }
            }
            else if (PlayerControl.LocalPlayer.Is(ObjectifierEnum.Mafia) && !ConstantVariables.DeadSeeEverything)
            {
                var mafia = localinfo[3] as Mafia;

                if (player.Is(ObjectifierEnum.Mafia) && player.TargetPlayerId != PlayerControl.LocalPlayer.PlayerId)
                {
                    name += $" {mafia.ColoredSymbol}";

                    if (CustomGameOptions.MafiaRoles)
                    {
                        var role = info[0] as Role;
                        color = role.Color;
                        name += $"\n{role.Name}";
                        roleRevealed = true;

                        if (PlayerControl.LocalPlayer.Is(RoleEnum.Consigliere))
                        {
                            var consigliere = localinfo[0] as Consigliere;

                            if (consigliere.Investigated.Contains(player.TargetPlayerId))
                                consigliere.Investigated.Remove(player.TargetPlayerId);
                        }
                    }
                }
            }

            if (PlayerControl.LocalPlayer.Is(AbilityEnum.Snitch) && CustomGameOptions.SnitchSeestargetsInMeeting && !(PlayerControl.LocalPlayer.Data.IsDead &&
                CustomGameOptions.DeadSeeEverything) && player != PlayerControl.LocalPlayer)
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

            if (PlayerControl.LocalPlayer.GetFaction() == player.GetFaction() && player.TargetPlayerId != PlayerControl.LocalPlayer.PlayerId && (player.GetFaction() == Faction.Intruder ||
                player.GetFaction() == Faction.Syndicate) && !ConstantVariables.DeadSeeEverything)
            {
                var role = info[0] as Role;

                if (CustomGameOptions.FactionSeeRoles)
                {
                    color = role.Color;
                    name += $"\n{role.Name}";
                    roleRevealed = true;

                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Consigliere))
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

            if (PlayerControl.LocalPlayer.Is(Faction.Syndicate) && player == Role.DriveHolder)
                name += " <color=#008000FF>Δ</color>";

            if (Role.GetRoles<Revealer>(RoleEnum.Revealer).Any(x => x.CompletedTasks) && PlayerControl.LocalPlayer.Is(Faction.Crew))
            {
                var role = info[0] as Role;

                if (CustomGameOptions.SnitchSeesRoles)
                {
                    if (player.Is(Faction.Syndicate) || player.Is(Faction.Intruder) || (player.Is(Faction.Neutral) && CustomGameOptions.RevealerRevealsNeutrals) ||
                        (player.Is(Faction.Crew) && CustomGameOptions.RevealerRevealsCrew))
                    {
                        color = role.Color;
                        name += $"\n{role.Name}";
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

            if (player.TargetPlayerId == PlayerControl.LocalPlayer.PlayerId && !player.AmDead)
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

            if (ConstantVariables.DeadSeeEverything)
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

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Consigliere))
                {
                    var consigliere = localinfo[0] as Consigliere;
                    consigliere.Investigated.Clear();
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Inspector))
                {
                    var inspector = localinfo[0] as Inspector;
                    inspector.Inspected.Clear();
                }
            }

            if (player.IsMarked())
                name += " <color=#F1C40FFF>χ</color>";

            if (ConstantVariables.DeadSeeEverything || player.TargetPlayerId == PlayerControl.LocalPlayer.PlayerId)
            {
                if (info[3] != null)
                {
                    var objectifier = info[3] as Objectifier;

                    if (objectifier.ObjectifierType != ObjectifierEnum.None && !objectifier.Hidden)
                        name += $" {objectifier.ColoredSymbol}";
                }

                var role = info[0] as Role;
                color = role.Color;
                name += $"\n{role.Name}";
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

                var swapPlayer1 = Utils.PlayerByVoteArea(role.Swap1);
                var swapPlayer2 = Utils.PlayerByVoteArea(role.Swap2);

                if (swapPlayer1 == null || swapPlayer2 == null || swapPlayer1.Data.IsDead || swapPlayer1.Data.Disconnected || swapPlayer2.Data.IsDead || swapPlayer2.Data.Disconnected)
                    continue;

                var pool1 = role.Swap1.PlayerIcon.transform;
                var name1 = role.Swap1.NameText.transform;
                var background1 = role.Swap1.Background.transform;
                var mask1 = role.Swap1.MaskArea.transform;
                var whiteBackground1 = role.Swap1.PlayerButton.transform;
                var pooldest1 = (Vector2)pool1.position;
                var namedest1 = (Vector2)name1.position;
                var backgroundDest1 = (Vector2)background1.position;
                var whiteBackgroundDest1 = (Vector2)whiteBackground1.position;
                var maskdest1 = (Vector2)mask1.position;

                var pool2 = role.Swap2.PlayerIcon.transform;
                var name2 = role.Swap2.NameText.transform;
                var background2 = role.Swap2.Background.transform;
                var mask2 = role.Swap2.MaskArea.transform;
                var whiteBackground2 = role.Swap2.PlayerButton.transform;

                var pooldest2 = (Vector2)pool2.position;
                var namedest2 = (Vector2)name2.position;
                var backgrounddest2 = (Vector2)background2.position;
                var maskdest2 = (Vector2)mask2.position;

                var whiteBackgroundDest2 = (Vector2)whiteBackground2.position;

                var duration = 1f / Ability.GetAbilities(AbilityEnum.Swapper).Count;

                Coroutines.Start(Slide2D(pool1, pooldest1, pooldest2, duration));
                Coroutines.Start(Slide2D(pool2, pooldest2, pooldest1, duration));
                Coroutines.Start(Slide2D(name1, namedest1, namedest2, duration));
                Coroutines.Start(Slide2D(name2, namedest2, namedest1, duration));
                Coroutines.Start(Slide2D(mask1, maskdest1, maskdest2, duration));
                Coroutines.Start(Slide2D(mask2, maskdest2, maskdest1, duration));
                Coroutines.Start(Slide2D(whiteBackground1, whiteBackgroundDest1, whiteBackgroundDest2, duration));
                Coroutines.Start(Slide2D(whiteBackground2, whiteBackgroundDest2, whiteBackgroundDest1, duration));
                yield return new WaitForSeconds(duration);
            }
        }
    }
}