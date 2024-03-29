namespace TownOfUsReworked.Patches;

public static class MeetingPatches
{
    public static int MeetingCount;
    public static GameData.PlayerInfo Reported;
    private static PlayerControl Reporter;
    public static bool GivingAnnouncements;

    [HarmonyPatch(typeof(PlayerVoteArea), nameof(PlayerVoteArea.SetCosmetics))]
    public static class PlayerStates
    {
        public static void Postfix(PlayerVoteArea __instance)
        {
            if (CustomGameOptions.CamouflagedMeetings && HudHandler.Instance.IsCamoed)
            {
                __instance.Background.sprite = Ship.CosmeticsCache.GetNameplate("nameplate_NoPlate").Image;
                __instance.LevelNumberText.GetComponentInParent<SpriteRenderer>().enabled = false;
                __instance.LevelNumberText.GetComponentInParent<SpriteRenderer>().gameObject.SetActive(false);
            }
            else
            {
                if (ClientGameOptions.WhiteNameplates)
                    __instance.Background.sprite = Ship.CosmeticsCache.GetNameplate("nameplate_NoPlate").Image;

                if (ClientGameOptions.NoLevels)
                {
                    __instance.LevelNumberText.GetComponentInParent<SpriteRenderer>().enabled = false;
                    __instance.LevelNumberText.GetComponentInParent<SpriteRenderer>().gameObject.SetActive(false);
                }
            }
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.ReportDeadBody))]
    public static class SetReported
    {
        public static void Postfix(PlayerControl __instance, ref GameData.PlayerInfo target)
        {
            Reported = target;
            Reporter = __instance;
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public static class MeetingHUD_Start
    {
        public static void Postfix(MeetingHud __instance)
        {
            __instance.gameObject.AddComponent<MeetingPagingBehaviour>().Menu = __instance;
            ClientStuff.CloseMenus();
            CustomPlayer.Local.DisableButtons();
            Ash.DestroyAll();
            MeetingCount++;
            Coroutines.Start(Announcements());

            if (Role.ChaosDriveMeetingTimerCount < CustomGameOptions.ChaosDriveMeetingCount)
                Role.ChaosDriveMeetingTimerCount++;

            if ((Role.ChaosDriveMeetingTimerCount == CustomGameOptions.ChaosDriveMeetingCount || IsKilling) && !Role.SyndicateHasChaosDrive)
            {
                Role.SyndicateHasChaosDrive = true;
                RoleGen.AssignChaosDrive();
            }

            CachedFirstDead = null;
        }

        private static IEnumerator Announcements()
        {
            GivingAnnouncements = true;
            yield return Wait(5f);

            if (CustomGameOptions.GameAnnouncements)
            {
                PlayerControl check = null;

                if (Reported != null)
                {
                    var player = Reported.Object;
                    check = player;
                    var report = $"{player.name} was found dead last round.";
                    Run("<color=#6C29ABFF>》 Game Announcement 《</color>", report);
                    yield return Wait(2f);
                    report = CustomGameOptions.LocationReports && BodyLocations.TryGetValue(Reported.PlayerId, out var location) ? $"Their body was found in {location}." :
                        "It is unknown where they died.";
                    Run("<color=#6C29ABFF>》 Game Announcement 《</color>", report);
                    yield return Wait(2f);
                    var killer = PlayerById(KilledPlayers.Find(x => x.PlayerId == player.PlayerId).KillerId);
                    var killerRole = killer.GetRole();

                    if (CustomGameOptions.KillerReports == RoleFactionReports.Role)
                        report = $"They were killed by the <b>{killerRole}</b>.";
                    else if (CustomGameOptions.KillerReports == RoleFactionReports.Faction)
                        report = $"They were killed by the <b>{killerRole.FactionName}</b>.";
                    else
                        report = "They were killed by an unknown assailant.";

                    Run("<color=#6C29ABFF>》 Game Announcement 《</color>", report);
                    yield return Wait(2f);
                    var role = player.GetRole();

                    if (CustomGameOptions.RoleFactionReports == RoleFactionReports.Role)
                        report = $"They were the <b>{role}</b>.";
                    else if (CustomGameOptions.RoleFactionReports == RoleFactionReports.Faction)
                        report = $"They were the <b>{role.FactionName}</b>.";
                    else
                        report = $"We could not determine what {player.name} was.";

                    Run("<color=#6C29ABFF>》 Game Announcement 《</color>", report);
                }
                else
                    Run("<color=#6C29ABFF>》 Game Announcement 《</color>", "A meeting has been called.");

                yield return Wait(2f);

                foreach (var playerid in RecentlyKilled)
                {
                    if (playerid != check.PlayerId)
                    {
                        var player = PlayerById(playerid);
                        var report = $"{player} was found dead last round.";
                        Run("<color=#6C29ABFF>》 Game Announcement 《</color>", report);
                        yield return Wait(2f);
                        report = "It is unknown where they died.";
                        Run("<color=#6C29ABFF>》 Game Announcement 《</color>", report);
                        yield return Wait(2f);

                        var killer = PlayerById(KilledPlayers.Find(x => x.PlayerId == player.PlayerId).KillerId);
                        var killerRole = killer.GetRole();

                        if (Role.Cleaned.Contains(player.PlayerId))
                            report = "They were killed by an unknown assailant.";
                        else if (CustomGameOptions.KillerReports == RoleFactionReports.Role)
                            report = $"They were killed by the <b>{killerRole.Name}</b>.";
                        else if (CustomGameOptions.KillerReports == RoleFactionReports.Faction)
                            report = $"They were killed by the <b>{killerRole.FactionName}</b>.";
                        else
                            report = "They were killed by an unknown assailant.";

                        Run("<color=#6C29ABFF>》 Game Announcement 《</color>", report);
                        yield return Wait(2f);
                        var role = player.GetRole();

                        if (Role.Cleaned.Contains(player.PlayerId))
                            report = $"We could not determine what {player} was.";
                        else if (CustomGameOptions.RoleFactionReports == RoleFactionReports.Role)
                            report = $"They were the <b>{role}</b>.";
                        else if (CustomGameOptions.RoleFactionReports == RoleFactionReports.Faction)
                            report = $"They were the <b>{role.FactionName}</b>.";
                        else
                            report = $"We could not determine what {player} was.";

                        Run("<color=#6C29ABFF>》 Game Announcement 《</color>", report);
                        yield return Wait(2f);
                    }
                }

                foreach (var player in DisconnectHandler.Disconnected)
                {
                    if (player != check.PlayerId)
                    {
                        Run("<color=#6C29ABFF>》 Game Announcement 《</color>", $"{PlayerById(player)} killed themselves last round.");
                        yield return Wait(2f);
                    }
                }

                foreach (var player in SetPostmortals.EscapedPlayers)
                {
                    if (player != check.PlayerId)
                    {
                        Run("<color=#6C29ABFF>》 Game Announcement 《</color>", $"{PlayerById(player)} accomplished their objective and escaped last round.");
                        yield return Wait(2f);
                    }
                }

                foreach (var player in SetPostmortals.MisfiredPlayers)
                {
                    if (player != check.PlayerId)
                    {
                        Run("<color=#6C29ABFF>》 Game Announcement 《</color>", $"{PlayerById(player)} was ejected for their misuse of power.");
                        yield return Wait(2f);
                    }
                }

                foreach (var player in SetPostmortals.MarkedPlayers)
                {
                    if (player != check.PlayerId)
                    {
                        Run("<color=#6C29ABFF>》 Game Announcement 《</color>", $"A Ghoul's curse forced {PlayerById(player)} to be ejected!");
                        yield return Wait(2f);
                    }
                }
            }

            var message = "";

            if (CustomGameOptions.SyndicateCount > 0)
            {
                if (Role.ChaosDriveMeetingTimerCount < CustomGameOptions.ChaosDriveMeetingCount - 1)
                    message = $"{CustomGameOptions.ChaosDriveMeetingCount - Role.ChaosDriveMeetingTimerCount} meetings remain till the <b>Syndicate</b> gets their hands on the Chaos Drive!";
                else if (Role.ChaosDriveMeetingTimerCount == CustomGameOptions.ChaosDriveMeetingCount - 1)
                    message = "This is the last meeting before the <b>Syndicate</b> gets their hands on the Chaos Drive!";
                else if (Role.ChaosDriveMeetingTimerCount == CustomGameOptions.ChaosDriveMeetingCount && MeetingCount == Role.ChaosDriveMeetingTimerCount)
                    message = "The <b>Syndicate</b> now possesses the Chaos Drive!";
                else
                    message = "The <b>Syndicate</b> possesses the Chaos Drive.";

                Run("<color=#6C29ABFF>》 Game Announcement 《</color>", message);

                yield return Wait(2f);
            }

            if (PlayerLayer.GetLayers<Overlord>().Any(x => x.Alive))
            {
                if (MeetingCount == CustomGameOptions.OverlordMeetingWinCount - 1)
                    message = "This is the last meeting to find and kill the <b>Overlord</b>. Should you fail, you will all lose!";
                else if (MeetingCount < CustomGameOptions.OverlordMeetingWinCount - 1)
                    message = "There seems to be an <b>Overlord</b> bent on dominating the mission! Kill them before they are successful!";

                if (message != "")
                    Run("<color=#6C29ABFF>》 Game Announcement 《</color>", message);

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
                            message = $"{knight.name} was knighted by a <b>Monarch</b>!";
                            Run("<color=#6C29ABFF>》 Game Announcement 《</color>", message);
                            knighted.Add(id);
                        }
                    }

                    yield return Wait(2f);
                }

                monarch.ToBeKnighted.Clear();
            }

            foreach (var layer in PlayerLayer.LocalLayers)
            {
                layer.OnMeetingStart(Meeting);

                if (layer.Player == Reporter)
                    layer.OnBodyReport(Reported);
            }

            RecentlyKilled.Clear();
            Role.Cleaned.Clear();
            Reported = null;
            DisconnectHandler.Disconnected.Clear();
            GivingAnnouncements = false;
            yield break;
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Close))]
    public static class MeetingHud_Close
    {
        public static void Postfix(MeetingHud __instance)
        {
            CustomPlayer.Local.EnableButtons();
            ButtonUtils.Reset(CooldownType.Meeting);
            PlayerLayer.LocalLayers.ForEach(x => x?.OnMeetingEnd(__instance));
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.VotingComplete))]
    public static class VotingComplete
    {
        public static void Postfix(MeetingHud __instance, ref GameData.PlayerInfo exiled, ref bool tie)
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
        public static void Postfix(MeetingHud __instance, ref int suspectStateIdx)
        {
            var id = suspectStateIdx;
            PlayerLayer.LocalLayers.ForEach(x => x?.SelectVote(__instance, id));
        }
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
                PlayerLayer.GetLayers<Politician>().ForEach(x => CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, x, PoliticianActionsRPC.Remove, x.ExtraVotes.ToArray()));
            }

            return false;
        }
    }

    private static IEnumerator Slide2D(Transform target, Vector2 source, Vector2 dest, float duration)
    {
        var temp = (Vector3)default;
        temp.z = target.position.z;

        for (var time = 0f; time < duration; time += Time.deltaTime)
        {
            var t = time / duration;
            temp.x = Mathf.SmoothStep(source.x, dest.x, t);
            temp.y = Mathf.SmoothStep(source.y, dest.y, t);
            target.position = temp;
            yield return EndFrame();
        }

        temp.x = dest.x;
        temp.y = dest.y;
        target.position = temp;
        yield break;
    }

    private static IEnumerator PerformSwaps()
    {
        foreach (var role in PlayerLayer.GetLayers<Swapper>())
        {
            if (!role.Alive || role.Swap1 == null || role.Swap2 == null)
                continue;

            var swapPlayer1 = PlayerByVoteArea(role.Swap1);
            var swapPlayer2 = PlayerByVoteArea(role.Swap2);

            if (swapPlayer1.HasDied() || swapPlayer2.HasDied())
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
            var votes1 = new List<Transform>();

            for (var childI = 0; childI < role.Swap1.transform.childCount; childI++)
            {
                if (role.Swap1.transform.GetChild(childI).gameObject.name == "playerVote(Clone)")
                    votes1.Add(role.Swap1.transform.GetChild(childI));
            }

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
            var level2 = role.Swap2.LevelNumberText.transform;
            var cb2 = role.Swap2.ColorBlindName.transform;
            var overlay2 = role.Swap2.Overlay.transform;
            var report2 = role.Swap2.Megaphone.transform;
            var votes2 = new List<Transform>();

            for (var childI = 0; childI < role.Swap2.transform.childCount; childI++)
            {
                if (role.Swap2.transform.GetChild(childI).gameObject.name == "playerVote(Clone)")
                    votes2.Add(role.Swap2.transform.GetChild(childI));
            }

            var pooldest2 = (Vector2)pool2.position;
            var namedest2 = (Vector2)name2.position;
            var backgrounddest2 = (Vector2)background2.position;
            var whiteBackgroundDest2 = (Vector2)whiteBackground2.position;
            var maskdest2 = (Vector2)mask2.position;
            var leveldest2 = (Vector2)level2.position;
            var cbdest2 = (Vector2)cb2.position;
            var overlaydest2 = (Vector2)overlay2.position;
            var reportdest2 = (Vector2)report2.position;

            foreach (var vote in votes2)
                vote.GetComponent<SpriteRenderer>().material.SetInt(PlayerMaterial.MaskLayer, role.Swap1.MaskLayer);

            foreach (var vote in votes1)
                vote.GetComponent<SpriteRenderer>().material.SetInt(PlayerMaterial.MaskLayer, role.Swap2.MaskLayer);

            var duration = 1f / (PlayerLayer.GetLayers<Swapper>().Count + 1);

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

            yield return Wait(duration);
        }

        yield break;
    }

    private static Dictionary<byte, int> CalculateAllVotes(this MeetingHud __instance, out bool tie, out KeyValuePair<byte, int> max)
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
            if (role.Revealed)
            {
                if (dictionary.TryGetValue(VoteAreaByPlayer(role.Player).VotedFor, out var num))
                    dictionary[VoteAreaByPlayer(role.Player).VotedFor] = num + CustomGameOptions.MayorVoteCount;
                else
                    dictionary[VoteAreaByPlayer(role.Player).VotedFor] = 1 + CustomGameOptions.MayorVoteCount;
            }
        }

        var knighted = new List<byte>();

        foreach (var role in PlayerLayer.GetLayers<Monarch>())
        {
            foreach (var id in role.Knighted)
            {
                if (!knighted.Contains(id))
                {
                    var area = VoteAreaById(id);

                    if (dictionary.TryGetValue(area.VotedFor, out var num))
                        dictionary[area.VotedFor] = num + CustomGameOptions.KnightVoteCount;
                    else
                        dictionary[area.VotedFor] = 1 + CustomGameOptions.KnightVoteCount;

                    knighted.Add(id);
                }
            }
        }

        foreach (var swapper in PlayerLayer.GetLayers<Swapper>())
        {
            if (swapper.Player.HasDied() || swapper.Swap1 == null || swapper.Swap2 == null)
                continue;

            var swapPlayer1 = PlayerByVoteArea(swapper.Swap1);
            var swapPlayer2 = PlayerByVoteArea(swapper.Swap2);

            if (swapPlayer1 == null || swapPlayer2 == null || swapPlayer1.HasDied() || swapPlayer2.HasDied())
                continue;

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
                if (!player.DidVote || player.AmDead || player.VotedFor == PlayerVoteArea.MissedVote || player.VotedFor == PlayerVoteArea.DeadVote)
                    continue;

                if (PlayerByVoteArea(player).Is(LayerEnum.Tiebreaker))
                {
                    if (dictionary.TryGetValue(player.VotedFor, out var num))
                        dictionary[player.VotedFor] = num + 1;
                    else
                        dictionary[player.VotedFor] = 1;
                }
            }
        }

        dictionary.MaxPair(out tie);
        return dictionary;
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