namespace TownOfUsReworked.Patches.Gameplay;

[HarmonyPatch(typeof(MeetingHud))]
public static class MeetingPatches
{
    public static int MeetingCount;
    public static NetworkedPlayerInfo Reported;
    public static PlayerControl Reporter;
    public static bool GivingAnnouncements;

    [HarmonyPatch(nameof(MeetingHud.Confirm))]
    public static void Postfix(MeetingHud __instance, byte suspectStateIdx)
    {
        if (IsLocalGame() && TownOfUsReworked.MciActive && TownOfUsReworked.SameVote.Value)
            __instance.playerStates.Do(x => __instance.CmdCastVote(x.TargetPlayerId, suspectStateIdx));
    }

    public static Sprite Cache;

    [HarmonyPatch(nameof(MeetingHud.SetForegroundForDead)), HarmonyPrefix]
    public static void SetForegroundForDeadPrefix(MeetingHud __instance) => Cache ??= __instance.Glass.sprite;

    [HarmonyPatch(nameof(MeetingHud.Start)), HarmonyPostfix]
    public static void StartPostfix(MeetingHud __instance)
    {
        __instance.AddComponent<MeetingPagingBehaviour>().Menu = __instance;
        Client.Instance.CloseMenus();
        LocalPlayer.DisableButtons();

        Ash.AllPiles.ForEach(x => x?.gameObject?.Destroy());
        Ash.AllPiles.Clear();

        MeetingCount++;

        if ((MeetingCount == SyndicateSettings.ChaosDriveMeetingCount || SyndicateSettings.AssignOnGameStart) && !Syndicate.SyndicateHasChaosDrive)
            AssignChaosDrive();

        Coroutines.Start(Announcements());
        CachedFirstDead = null;
        GameTimerHandler.Instance?.UpdateTimer();
    }

    public static float MeetingStartTime;

    [HarmonyPatch(nameof(MeetingHud.Start))]
    public static void Prefix() => MeetingStartTime = Time.time;

    private static IEnumerator Announcements()
    {
        GivingAnnouncements = true;
        yield return Wait(5f);

        if (GameAnnouncementSettings.GameAnnouncements)
        {
            PlayerControl check = null;

            if (Reported)
            {
                var player = Reported.Object;
                check = player;
                var report = $"{player.name} was found dead last round.";
                Run("<#6C29ABFF>》 Game Announcement 《</color>", report);
                yield return Wait(2f);
                report = GameAnnouncementSettings.LocationReports && BodyLocations.TryGetValue(Reported.PlayerId, out var location) ? $"Their body was found in {location}." :
                    "It is unknown where they died.";
                Run("<#6C29ABFF>》 Game Announcement 《</color>", report);
                yield return Wait(2f);
                var killerRole = PlayerById(KilledPlayers.Find(x => x.PlayerId == player.PlayerId).KillerId).GetRole();
                report = GameAnnouncementSettings.KillerReports switch
                {
                    RoleFactionReports.Both => $"They were killed by member of the {killerRole.FactionName}, the {killerRole}.",
                    RoleFactionReports.Role => $"They were killed by the {killerRole}.",
                    RoleFactionReports.Faction => $"They were killed by a member of the {killerRole.FactionName}.",
                    _ => "They were killed by an unknown assailant."
                };

                Run("<#6C29ABFF>》 Game Announcement 《</color>", report);
                yield return Wait(2f);
                var role = player.GetRole();
                report = GameAnnouncementSettings.RoleFactionReports switch
                {
                    RoleFactionReports.Both => $"They were a member of the {role.FactionName}, the {role}.",
                    RoleFactionReports.Role => $"They were the {role}.",
                    RoleFactionReports.Faction => $"They were a member of the {role.FactionName}.",
                    _ => $"We could not determine what {player.name} was."
                };

                Run("<#6C29ABFF>》 Game Announcement 《</color>", report);
            }
            else
                Run("<#6C29ABFF>》 Game Announcement 《</color>", "A meeting has been called.");

            yield return Wait(2f);

            foreach (var playerid in RecentlyKilled)
            {
                if (playerid == check?.PlayerId)
                    continue;

                var player = PlayerById(playerid);
                var report = $"{player.name} was found dead last round.";
                Run("<#6C29ABFF>》 Game Announcement 《</color>", report);
                yield return Wait(2f);
                report = "It is unknown where they died.";
                Run("<#6C29ABFF>》 Game Announcement 《</color>", report);
                yield return Wait(2f);

                var killerRole = PlayerById(KilledPlayers.Find(x => x.PlayerId == player.PlayerId).KillerId).GetRole();

                if (Cleaned.Contains(player.PlayerId))
                    report = "They were killed by an unknown assailant.";
                else
                {
                    report = GameAnnouncementSettings.KillerReports switch
                    {
                        RoleFactionReports.Role => $"They were killed by the {killerRole.Name}.",
                        RoleFactionReports.Faction => $"They were killed by the {killerRole.FactionName}.",
                        _ => "They were killed by an unknown assailant."
                    };
                }

                Run("<#6C29ABFF>》 Game Announcement 《</color>", report);
                yield return Wait(2f);
                var role = player.GetRole();

                if (Cleaned.Contains(player.PlayerId))
                    report = $"We could not determine what {player.name} was.";
                else
                {
                    report = GameAnnouncementSettings.RoleFactionReports switch
                    {
                        RoleFactionReports.Role => $"They were the {role}.",
                        RoleFactionReports.Faction => $"They were the {role.FactionName}.",
                        _ => $"We could not determine what {player.name} was."
                    };
                }

                Run("<#6C29ABFF>》 Game Announcement 《</color>", report);
                yield return Wait(2f);
            }
        }

        var message = string.Empty;

        if (SyndicateSettings.SyndicateCount > 0)
        {
            if (MeetingCount < SyndicateSettings.ChaosDriveMeetingCount - 1)
                message = $"{SyndicateSettings.ChaosDriveMeetingCount - MeetingCount} meetings remain till the Syndicate gets their hands on the Chaos Drive!";
            else if (MeetingCount == SyndicateSettings.ChaosDriveMeetingCount - 1)
                message = "This is the last meeting before the Syndicate gets their hands on the Chaos Drive!";
            else if (MeetingCount == SyndicateSettings.ChaosDriveMeetingCount)
                message = "The Syndicate now possesses the Chaos Drive!";
            else
                message = "The Syndicate possesses the Chaos Drive.";

            Run("<#6C29ABFF>》 Game Announcement 《</color>", message);

            yield return Wait(2f);
        }

        if (PlayerLayer.GetLayers<Overlord>().Any(x => x.Alive))
        {
            if (MeetingCount == Overlord.OverlordMeetingWinCount - 1)
                message = "This is the last meeting to find and kill the Overlord. Should you fail, you will all lose!";
            else if (MeetingCount < Overlord.OverlordMeetingWinCount - 1)
                message = "There seems to be an Overlord bent on dominating the mission! Kill them before they are successful!";

            if (!IsNullEmptyOrWhiteSpace(message))
                Run("<#6C29ABFF>》 Game Announcement 《</color>", message);

            yield return Wait(2f);
        }

        var knighted = new List<byte>();

        foreach (var monarch in PlayerLayer.GetLayers<Monarch>())
        {
            foreach (var id in monarch.ToBeKnighted)
            {
                monarch.Knighted.Add(id);

                if (!knighted.Contains(id))
                {
                    var knight = PlayerById(id);

                    if (!knight.HasDied())
                    {
                        message = $"{knight.name} was knighted by a Monarch!";
                        Run("<#6C29ABFF>》 Game Announcement 《</color>", message);
                        knighted.Add(id);
                    }
                }

                yield return Wait(2f);
            }

            monarch.ToBeKnighted.Clear();
        }

        foreach (var layer in PlayerLayer.LocalLayers())
        {
            if (layer.Player == Reporter)
                layer.OnBodyReport(Reported);
        }

        RecentlyKilled.Clear();
        Cleaned.Clear();
        Reported = null;
        GivingAnnouncements = false;
    }

    [HarmonyPatch(nameof(MeetingHud.Close)), HarmonyPostfix]
    public static void ClosePostfix(MeetingHud __instance)
    {
        LocalPlayer.EnableButtons();
        PlayerLayer.GetLayers<Werewolf>().Do(x => x.Rounds++);

        if (LayerHandler.Handlers.TryGetValue(LocalPlayer.PlayerId, out var handler))
            handler.OnMeetingEnd(__instance);
    }

    [HarmonyPatch(nameof(MeetingHud.Update)), HarmonyPostfix]
    public static void UpdatePostfix(MeetingHud __instance)
    {
        if (IsCustomHnS() || IsTaskRace())
            return;

        // Deactivate skip Button if skipping an emergency meeting is disabled
        __instance.SkipVoteButton.gameObject.SetActive(!((Reported is null && VotingOptions.NoSkipping == DisableSkipButtonMeetings.Emergency) || (VotingOptions.NoSkipping ==
            DisableSkipButtonMeetings.Always)) && __instance.state == MeetingHud.VoteStates.NotVoted && !LocalPlayer.HasDied());

        if (LayerHandler.Handlers.TryGetValue(LocalPlayer.PlayerId, out var handler))
            handler.UpdateMeeting(__instance);
    }

    [HarmonyPatch(nameof(MeetingHud.VotingComplete))]
    public static void Postfix(NetworkedPlayerInfo exiled, bool tie)
    {
        var exiledString = !exiled ? "null" : exiled.PlayerName;
        Info($"Exiled PlayerName = {exiledString}");
        Info($"Was a tie = {tie}");
        Coroutines.Start(PerformSwaps());

        foreach (var role in PlayerLayer.GetLayers<Role>())
        {
            if (role.Type is Layer.Phantom or Layer.Ghoul or Layer.Banshee or Layer.Revealer or Layer.GuardianAngel or Layer.Jester || !role.Dead)
                continue;

            role.TrulyDead = true;
        }
    }

    [HarmonyPatch(nameof(MeetingHud.Select))]
    public static void Postfix(MeetingHud __instance, int suspectStateIdx) => PlayerLayer.LocalLayers().Do(x => x?.SelectVote(__instance, suspectStateIdx));

    [HarmonyPatch(nameof(MeetingHud.ClearVote)), HarmonyPostfix]
    public static void ClearVotePostfix(MeetingHud __instance) => PlayerLayer.LocalLayers().Do(x => x?.ClearVote(__instance));

    private static bool CheckVoted(PlayerVoteArea playerVoteArea)
    {
        CheckVotedVoid(playerVoteArea);
        return true;
    }

    private static void CheckVotedVoid(PlayerVoteArea playerVoteArea)
    {
        if (playerVoteArea.AmDead || playerVoteArea.DidVote)
            return;

        var playerInfo = GameData.Instance.GetPlayerById(playerVoteArea.TargetPlayerId);

        if (!playerInfo || !playerInfo.IsDead)
            return;

        playerVoteArea.VotedFor = PlayerVoteArea.DeadVote;
        playerVoteArea.SetDead(false, true);
    }

    [HarmonyPatch(nameof(MeetingHud.CheckForEndVoting)), HarmonyPrefix]
    public static bool CheckForEndVotingPrefix(MeetingHud __instance)
    {
        if (__instance.playerStates.All(ps => ps.AmDead || (ps.DidVote && CheckVoted(ps))))
        {
            __instance.CalculateAllVotes(out var tie, out var maxIdx);
            var array = new Il2CppStructArray<MeetingHud.VoterState>(__instance.playerStates.Length);
            var exiled = tie ? null : GameData.Instance.GetPlayerById(maxIdx.Key);

            for (var i = 0; i < __instance.playerStates.Length; i++)
            {
                var playerVoteArea = __instance.playerStates[i];
                array[i] = new()
                {
                    VoterId = playerVoteArea.TargetPlayerId,
                    VotedForId = playerVoteArea.VotedFor
                };
            }

            __instance.RpcVotingComplete(array, exiled, tie);
            PlayerLayer.GetLayers<Politician>().Do(x => x.PerformRpcAction(PoliticianActionsRpc.Remove, x.ExtraVotes.ToArray()));
        }

        return false;
    }

    // Thanks twix
    [HarmonyPatch(nameof(MeetingHud.CoIntro))]
    public static void Prefix(MeetingHud __instance, NetworkedPlayerInfo reportedBody, Il2CppReferenceArray<NetworkedPlayerInfo> deadBodies)
    {
        if (!GameAnnouncementSettings.IndicateReportedBodies)
            return;

        foreach (var player in __instance.playerStates)
        {
            if (deadBodies.All(x => x.PlayerId != player.TargetPlayerId))
                continue;

            player.Megaphone.gameObject.SetActive(true);
            player.Megaphone.enabled = true;
            player.Megaphone.transform.localEulerAngles = Vector3.zero;
            player.Megaphone.transform.localScale = Vector3.one;

            if (player.TargetPlayerId == reportedBody.PlayerId)
                continue;

            player.Megaphone.sprite = GameManager.Instance.GetDeadBody(RoleManager.Instance.GetRole(RoleTypes.Impostor)).bodyRenderers[0].sprite;
            player.Megaphone.transform.localScale = new(0.3f, 0.3f, 0.3f);
            player.Megaphone.transform.localPosition -= new Vector3(0.2f, 0, 0);
        }
    }

    private static IEnumerator Slide2D(Transform target, Vector3 source, Vector3 dest, float duration) => PerformTimedAction(duration, p => target.position = Vector3.Lerp(source, dest, p));

    private static IEnumerator PerformSwaps()
    {
        var swappers = PlayerLayer.GetLayers<Swapper>().Where(x => x.Alive && !PlayerByVoteArea(x.Swap1).HasDied() && !PlayerByVoteArea(x.Swap2).HasDied());
        var duration = 1.5f / (swappers.Count() + 1);

        foreach (var role in swappers)
        {
            var pool1 = role.Swap1.PlayerIcon.transform;
            var name1 = role.Swap1.NameText.transform;
            var background1 = role.Swap1.Background.transform;
            var mask1 = role.Swap1.MaskArea.transform;
            var whiteBackground1 = role.Swap1.PlayerButton.transform;
            var level1 = role.Swap1.LevelNumberText.transform;
            var cb1 = role.Swap1.ColorBlindName.transform;
            var overlay1 = role.Swap1.Overlay.transform;
            var report1 = role.Swap1.Megaphone.transform;
            var votes1 = new List<Transform>();

            for (var childI = 0; childI < role.Swap1.transform.childCount; childI++)
            {
                if (role.Swap1.transform.GetChild(childI).name == "playerVote(Clone)")
                    votes1.Add(role.Swap1.transform.GetChild(childI));
            }

            var pooldest1 = pool1.position;
            var namedest1 = name1.position;
            var backgroundDest1 = background1.position;
            var whiteBackgroundDest1 = whiteBackground1.position;
            var maskdest1 = mask1.position;
            var leveldest1 = level1.position;
            var cbdest1 = cb1.position;
            var overlaydest1 = overlay1.position;
            var reportdest1 = report1.position;

            var pool2 = role.Swap2.PlayerIcon.transform;
            var name2 = role.Swap2.NameText.transform;
            var background2 = role.Swap2.Background.transform;
            var mask2 = role.Swap2.MaskArea.transform;
            var whiteBackground2 = role.Swap2.PlayerButton.transform;
            var level2 = role.Swap2.LevelNumberText.transform;
            var cb2 = role.Swap2.ColorBlindName.transform;
            var overlay2 = role.Swap2.Overlay.transform;
            var report2 = role.Swap2.Megaphone.transform;
            var votes2 = new List<Transform>();

            for (var childI = 0; childI < role.Swap2.transform.childCount; childI++)
            {
                if (role.Swap2.transform.GetChild(childI).name == "playerVote(Clone)")
                    votes2.Add(role.Swap2.transform.GetChild(childI));
            }

            var pooldest2 = pool2.position;
            var namedest2 = name2.position;
            var backgroundDest2 = background2.position;
            var whiteBackgroundDest2 = whiteBackground2.position;
            var maskdest2 = mask2.position;
            var leveldest2 = level2.position;
            var cbdest2 = cb2.position;
            var overlaydest2 = overlay2.position;
            var reportdest2 = report2.position;

            votes2.ForEach(x => x.GetComponent<SpriteRenderer>().material.SetInt(PlayerMaterial.MaskLayer, role.Swap1.MaskLayer));
            votes1.ForEach(x => x.GetComponent<SpriteRenderer>().material.SetInt(PlayerMaterial.MaskLayer, role.Swap2.MaskLayer));

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
            Coroutines.Start(Slide2D(background1, backgroundDest1, backgroundDest2, duration));
            Coroutines.Start(Slide2D(background2, backgroundDest2, backgroundDest1, duration));
            Coroutines.Start(Slide2D(whiteBackground1, whiteBackgroundDest1, whiteBackgroundDest2, duration));
            Coroutines.Start(Slide2D(whiteBackground2, whiteBackgroundDest2, whiteBackgroundDest1, duration));

            yield return Wait(duration);
        }
    }

    private static void CalculateAllVotes(this MeetingHud __instance, out bool tie, out KeyValuePair<byte, int> max)
    {
        var dictionary = new Dictionary<byte, int>();

        foreach (var playerVoteArea in __instance.playerStates)
        {
            if (!playerVoteArea.DidVote || playerVoteArea.AmDead || playerVoteArea.VotedFor == PlayerVoteArea.MissedVote || playerVoteArea.VotedFor == PlayerVoteArea.DeadVote)
                continue;

            if (dictionary.TryGetValue(playerVoteArea.VotedFor, out var num))
                dictionary[playerVoteArea.VotedFor] = num + 1;
            else
                dictionary[playerVoteArea.VotedFor] = 1;
        }

        foreach (var role in PlayerLayer.GetLayers<Politician>())
        {
            foreach (var number in role.ExtraVotes)
            {
                if (dictionary.TryGetValue(number, out var num))
                    dictionary[number] = num + 1;
                else
                    dictionary[number] = 1;
            }
        }

        foreach (var role in PlayerLayer.GetLayers<Mayor>())
        {
            var id = VoteAreaByPlayer(role.Player).VotedFor;

            if (dictionary.TryGetValue(id, out var num))
                dictionary[id] = num + Mayor.MayorVoteCount;
            else
                dictionary[id] = 1 + Mayor.MayorVoteCount;
        }

        var knighted = new HashSet<byte>();

        foreach (var role in PlayerLayer.GetLayers<Monarch>())
        {
            foreach (var area in from id in role.Knighted where knighted.Add(id) select VoteAreaById(id))
            {
                if (dictionary.TryGetValue(area.VotedFor, out var num))
                    dictionary[area.VotedFor] = num + Monarch.KnightVoteCount;
                else
                    dictionary[area.VotedFor] = 1 + Monarch.KnightVoteCount;
            }
        }

        foreach (var swapper in PlayerLayer.GetLayers<Swapper>().Where(x => x.Alive && !PlayerByVoteArea(x.Swap1).HasDied() && !PlayerByVoteArea(x.Swap2).HasDied()))
        {
            var swap1 = 0;
            var swap2 = 0;

            if (dictionary.TryGetValue(swapper.Swap1.TargetPlayerId, out var value))
                swap1 = value;

            if (dictionary.TryGetValue(swapper.Swap2.TargetPlayerId, out var value2))
                swap2 = value2;

            dictionary[swapper.Swap2.TargetPlayerId] = swap1;
            dictionary[swapper.Swap1.TargetPlayerId] = swap2;
        }

        max = dictionary.MaxPair(out tie);

        if (tie)
        {
            foreach (var player in __instance.playerStates)
            {
                if (!player.DidVote || player.AmDead || player.VotedFor == PlayerVoteArea.MissedVote || player.VotedFor == PlayerVoteArea.DeadVote || !PlayerByVoteArea(player).Is<Tiebreaker>())
                    continue;

                if (dictionary.TryGetValue(player.VotedFor, out var num))
                    dictionary[player.VotedFor] = num + 1;
                else
                    dictionary[player.VotedFor] = 1;
            }
        }

        dictionary.MaxPair(out tie);
    }

    private static KeyValuePair<byte, int> MaxPair(this Dictionary<byte, int> self, out bool tie)
    {
        tie = true;
        var result = new KeyValuePair<byte, int>(255, int.MinValue);

        foreach (var keyValuePair in self)
        {
            if (keyValuePair.Value > result.Value)
            {
                result = keyValuePair;
                tie = false;
            }
            else if (keyValuePair.Value == result.Value)
                tie = true;
        }

        return result;
    }
}