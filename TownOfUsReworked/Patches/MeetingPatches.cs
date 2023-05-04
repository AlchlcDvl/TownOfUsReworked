using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.CustomOptions;
using Hazel;
using Reactor.Utilities.Extensions;
using TownOfUsReworked.Data;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.Classes;
using TownOfUsReworked.PlayerLayers;
using System;
using System.Collections.Generic;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Object = UnityEngine.Object;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Crowded.Components;
using System.Collections;
using Reactor.Utilities;
using TownOfUsReworked.PlayerLayers.Objectifiers;
using System.Linq;
using TownOfUsReworked.PlayerLayers.Abilities;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch]
    public static class MeetingPatches
    {
        #pragma warning disable
        private static GameData.PlayerInfo VoteTarget;
        public static int MeetingCount;
        private static string Location = "";
        private static GameData.PlayerInfo Reported = null;
        public static bool GivingAnnouncements = false;
        #pragma warning restore

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

        [HarmonyPatch(typeof(RoomTracker), nameof(RoomTracker.FixedUpdate))]
        public static class Recordlocation
        {
            public static void Postfix(RoomTracker __instance) => Location = __instance.text.transform.localPosition.y != -3.25f ? __instance.text.text : "a hallway or somewhere outside.";
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdReportDeadBody))]
        [HarmonyPriority(Priority.First)]
        public static class SetReported
        {
            public static void Postfix([HarmonyArgument(0)] GameData.PlayerInfo target) => Reported = target;
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
                Coroutines.Start(Announcements(Reported, __instance));

                foreach (var player in PlayerControl.AllPlayerControls)
                    player?.MyPhysics?.ResetAnimState();

                foreach (var role in Role.AllRoles)
                    role.Zooming = false;

                if (Role.ChaosDriveMeetingTimerCount < CustomGameOptions.ChaosDriveMeetingCount)
                    Role.ChaosDriveMeetingTimerCount++;

                if (Role.ChaosDriveMeetingTimerCount == CustomGameOptions.ChaosDriveMeetingCount && !Role.SyndicateHasChaosDrive)
                {
                    Role.SyndicateHasChaosDrive = true;
                    RoleGen.AssignChaosDrive();
                }

                foreach (var layer in PlayerLayer.GetLayers(PlayerControl.LocalPlayer))
                    layer?.OnMeetingStart(__instance);
            }

            private static IEnumerator Announcements(GameData.PlayerInfo target, MeetingHud __instance)
            {
                foreach (var layer in PlayerLayer.GetLayers(PlayerControl.LocalPlayer))
                    layer?.OnBodyReport(target);

                yield return new WaitForSeconds(5f);

                GivingAnnouncements = true;
                var extraTime = 0f;

                if (CustomGameOptions.GameAnnouncements)
                {
                    PlayerControl check = null;

                    if (target != null)
                    {
                        var player = target.Object;
                        check = player;
                        var report = $"{player.name} was found dead last round.";
                        HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, report);

                        yield return new WaitForSeconds(2f);

                        extraTime += 2;

                        if (CustomGameOptions.LocationReports)
                            report = $"Their body was found in {Location}.";
                        else
                            report = "It is unknown where they died.";

                        HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, report);

                        yield return new WaitForSeconds(2f);

                        extraTime += 2;
                        var killer = Utils.PlayerById(Murder.KilledPlayers.Find(x => x.PlayerId == player.PlayerId).KillerId);
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

                        extraTime += 2;
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
                    extraTime += 2;

                    foreach (var player in Utils.RecentlyKilled)
                    {
                        if (player != check)
                        {
                            var report = $"{player.name} was found dead last round.";
                            HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, report);

                            yield return new WaitForSeconds(2f);

                            extraTime += 2;

                            report = "It is unknown where they died.";
                            HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, report);

                            yield return new WaitForSeconds(2f);

                            extraTime += 2;

                            var killer = Utils.PlayerById(Murder.KilledPlayers.Find(x => x.PlayerId == player.PlayerId).KillerId);
                            var killerRole = Role.GetRole(killer);

                            if (Role.Cleaned.Contains(player))
                                report = "They were killed by an unknown assailant.";
                            else if (CustomGameOptions.KillerReports == RoleFactionReports.Role)
                                report = $"They were killed by a(n) {killerRole.Name}.";
                            else if (CustomGameOptions.KillerReports == RoleFactionReports.Faction)
                                report = $"They were killed by a member of the {killerRole.FactionName}.";
                            else
                                report = "They were killed by an unknown assailant.";

                            HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, report);

                            yield return new WaitForSeconds(2f);

                            extraTime += 2;

                            var role = Role.GetRole(player);

                            if (Role.Cleaned.Contains(player))
                                report = $"We could not determine what {player.name} was.";
                            else if (CustomGameOptions.RoleFactionReports == RoleFactionReports.Role)
                                report = $"They were a(n) {role.Name}.";
                            else if (CustomGameOptions.RoleFactionReports == RoleFactionReports.Faction)
                                report = $"They were from the {role.FactionName} faction.";
                            else
                                report = $"We could not determine what {player.name} was.";

                            HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, report);

                            yield return new WaitForSeconds(2f);

                            extraTime += 2;
                        }
                    }

                    foreach (var player in DisconnectHandler.Disconnected)
                    {
                        if (player != check)
                        {
                            HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, $"{player.name} escaped the ship last round.");

                            yield return new WaitForSeconds(2f);

                            extraTime += 2;
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

                extraTime += 2;

                if (Objectifier.GetObjectifiers<Overlord>(ObjectifierEnum.Overlord).Any(x => x.IsAlive))
                {
                    if (MeetingCount == CustomGameOptions.OverlordMeetingWinCount - 1)
                        message = "This is the last meeting to find and kill the Overlord. Should you fail, you will all lose!";
                    else if (MeetingCount < CustomGameOptions.OverlordMeetingWinCount - 1)
                        message = "There seems to be an Overlord bent on dominating the mission! Kill them before they are successful!";

                    if (message != "")
                        HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, message);

                    yield return new WaitForSeconds(2f);

                    extraTime += 2;
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

                __instance.discussionTimer += extraTime;
                Utils.RecentlyKilled.Clear();
                Role.Cleaned.Clear();
                GivingAnnouncements = false;
                Reported = null;
                DisconnectHandler.Disconnected.Clear();
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Close))]
        public static class MeetingHud_Close
        {
            public static void Postfix(MeetingHud __instance)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.MeetingStart, SendOption.Reliable);
                AmongUsClient.Instance.FinishRpcImmediately(writer);

                foreach (var body in Object.FindObjectsOfType<DeadBody>())
                    body.gameObject.Destroy();

                foreach (var player in PlayerControl.AllPlayerControls)
                    player.MyPhysics.ResetAnimState();

                foreach (var layer in PlayerLayer.GetLayers(PlayerControl.LocalPlayer))
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
                    }
                }

                foreach (var player in __instance.playerStates)
                    (player.NameText.text, player.NameText.color) = UpdateGameName(player);

                foreach (var layer in PlayerLayer.GetLayers(PlayerControl.LocalPlayer))
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

                foreach (var layer in PlayerLayer.GetLayers(PlayerControl.LocalPlayer))
                    layer.VoteComplete(__instance);

                Coroutines.Start(PerformSwaps());

                foreach (var layer in PlayerLayer.GetLayers(PlayerControl.LocalPlayer))
                    layer?.OnMeetingEnd(__instance);
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Select))]
        public static class MeetingHudSelect
        {
            public static void Postfix(MeetingHud __instance, int __0)
            {
                foreach (var layer in PlayerLayer.GetLayers(PlayerControl.LocalPlayer))
                    layer.SelectVote(__instance, __0);
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.ClearVote))]
        public static class MeetingHudClearVote
        {
            public static void Postfix(MeetingHud __instance)
            {
                foreach (var layer in PlayerLayer.GetLayers(PlayerControl.LocalPlayer))
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
                __instance.TitleText.text = Object.FindObjectOfType<TranslationController>().GetString(StringNames.MeetingVotingResults, Array.Empty<Il2CppSystem.Object>());
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
                    GameOptionsManager.Instance.currentNormalGameOptions.AnonymousVotes = CustomGameOptions.PoliticianAnonymous;

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

                    GameOptionsManager.Instance.currentNormalGameOptions.AnonymousVotes = CustomGameOptions.AnonymousVoting;
                }

                /*foreach (var mayor in Role.GetRoles<Mayor>(RoleEnum.Mayor))
                {
                    var playerInfo = mayor.Player.Data;
                    GameOptionsManager.Instance.currentNormalGameOptions.AnonymousVotes = CustomGameOptions.MayorAnonymous;

                    if (mayor.Voted == PlayerVoteArea.HasNotVoted || mayor.Voted == PlayerVoteArea.MissedVote || mayor.Voted == PlayerVoteArea.DeadVote || !mayor.Revealed)
                        continue;

                    for (var j = 0; j < CustomGameOptions.MayorVoteCount; j++)
                    {
                        if (mayor.Voted == PlayerVoteArea.SkippedVote)
                        {
                            __instance.BloopAVoteIcon(playerInfo, amountOfSkippedVoters, __instance.SkippedVoting.transform);
                            amountOfSkippedVoters++;
                        }
                        else
                        {
                            for (var i = 0; i < __instance.playerStates.Length; i++)
                            {
                                var area = __instance.playerStates[i];

                                if (area.TargetPlayerId != mayor.Voted)
                                    continue;

                                __instance.BloopAVoteIcon(playerInfo, allNums[i], area.transform);
                                allNums[i]++;
                            }
                        }
                    }

                    GameOptionsManager.Instance.currentNormalGameOptions.AnonymousVotes = CustomGameOptions.AnonymousVoting;
                }*/

                return false;
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.BloopAVoteIcon))]
        public static class DeadSeeVoteColorsPatch
        {
            public static bool Prefix(MeetingHud __instance, [HarmonyArgument(0)] GameData.PlayerInfo voterPlayer, [HarmonyArgument(1)] int index, [HarmonyArgument(2)] Transform parent)
            {
                var spriteRenderer = Object.Instantiate(__instance.PlayerVotePrefab);
                var insiderFlag = false;
                var deadFlag = CustomGameOptions.DeadSeeEverything && PlayerControl.LocalPlayer.Data.IsDead;

                if (PlayerControl.LocalPlayer.Is(AbilityEnum.Insider))
                    insiderFlag = Role.GetRole(PlayerControl.LocalPlayer).TasksDone;

                if (GameOptionsManager.Instance.currentNormalGameOptions.AnonymousVotes && !(deadFlag || insiderFlag))
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

            if (player.CanDoTasks() && (PlayerControl.LocalPlayer.PlayerId == player.TargetPlayerId || (PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything)))
            {
                var role = info[0] as Role;
                name += $"{((DoUndo.IsCamoed && CustomGameOptions.MeetingColourblind) || PlayerControl.LocalPlayer.Data.IsDead ? name : "")} ({role.TasksCompleted}/{role.TotalTasks})";
                roleRevealed = true;
            }

            if (player.IsKnighted())
                name += "<color=#FF004EFF>κ</color>";

            if (player.Is(RoleEnum.Mayor) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything) && PlayerControl.LocalPlayer.PlayerId != player.TargetPlayerId)
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

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Coroner) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
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
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Consigliere) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
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
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.PromotedGodfather) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                var godfather = localinfo[0] as PromotedGodfather;

                if (godfather.IsConsig)
                {
                    if (godfather.Investigated.Contains(player.TargetPlayerId))
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

                if (ret.IsInsp)
                {
                    if (ret.Inspected.Contains(player.TargetPlayerId))
                    {
                        name += $"\n{player.GetInspResults()}";
                        color = ret.Color;
                        roleRevealed = true;
                    }
                }
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Arsonist) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                var arsonist = localinfo[0] as Arsonist;

                if (arsonist.DousedPlayers.Contains(player.TargetPlayerId))
                    name += " <color=#EE7600FF>Ξ</color>";
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Plaguebearer) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                var plaguebearer = localinfo[0] as Plaguebearer;

                if (plaguebearer.InfectedPlayers.Contains(player.TargetPlayerId))
                    name += " <color=#CFFE61FF>ρ</color>";
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Cryomaniac) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                var cryomaniac = localinfo[0] as Cryomaniac;

                if (cryomaniac.DousedPlayers.Contains(player.TargetPlayerId))
                    name += " <color=#642DEAFF>λ</color>";
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Framer) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                var framer = localinfo[0] as Framer;

                if (framer.Framed.Contains(player.TargetPlayerId))
                    name += " <color=#00FFFFFF>ς</color>";
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Executioner) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
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
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Guesser) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                var guesser = localinfo[0] as Guesser;

                if (player.TargetPlayerId == guesser.TargetPlayer.PlayerId)
                {
                    color = guesser.Color;
                    name += " <color=#EEE5BEFF>π</color>";
                }
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.GuardianAngel) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
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
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Whisperer) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
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
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Dracula) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
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
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Jackal) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
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
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Necromancer) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
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

            if (PlayerControl.LocalPlayer.IsBitten() && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
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
            else if (PlayerControl.LocalPlayer.IsRecruit() && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
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
            else if (PlayerControl.LocalPlayer.IsResurrected() && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
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
            else if (PlayerControl.LocalPlayer.IsPersuaded() && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
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

            if (PlayerControl.LocalPlayer.Is(ObjectifierEnum.Lovers) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
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
            else if (PlayerControl.LocalPlayer.Is(ObjectifierEnum.Rivals) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
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
            else if (PlayerControl.LocalPlayer.Is(ObjectifierEnum.Mafia) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
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
                player.GetFaction() == Faction.Syndicate) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
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

            if (PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything)
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

                foreach (var arsonist in Role.GetRoles<Arsonist>(RoleEnum.Arsonist))
                {
                    if (arsonist.DousedPlayers.Contains(player.TargetPlayerId))
                        name += " <color=#EE7600FF>Ξ</color>";
                }

                foreach (var plaguebearer in Role.GetRoles<Plaguebearer>(RoleEnum.Plaguebearer))
                {
                    if (plaguebearer.InfectedPlayers.Contains(player.TargetPlayerId))
                        name += " <color=#CFFE61FF>ρ</color>";
                }

                foreach (var cryomaniac in Role.GetRoles<Cryomaniac>(RoleEnum.Cryomaniac))
                {
                    if (cryomaniac.DousedPlayers.Contains(player.TargetPlayerId))
                        name += " <color=#642DEAFF>λ</color>";
                }

                foreach (var framer in Role.GetRoles<Framer>(RoleEnum.Framer))
                {
                    if (framer.Framed.Contains(player.TargetPlayerId))
                        name += " <color=#00FFFFFF>ς</color>";
                }

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

            if ((PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything) || player.TargetPlayerId == PlayerControl.LocalPlayer.PlayerId)
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
                player.ColorBlindName.transform.localPosition = new Vector3(-0.93f, -0.2f, -0.1f);

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

                var duration = 2f / Ability.GetAbilities(AbilityEnum.Swapper).Count;

                Coroutines.Start(Slide2D(pool1, pooldest1, pooldest2, duration));
                Coroutines.Start(Slide2D(pool2, pooldest2, pooldest1, duration));
                Coroutines.Start(Slide2D(name1, namedest1, namedest2, duration));
                Coroutines.Start(Slide2D(name2, namedest2, namedest1, duration));
                Coroutines.Start(Slide2D(mask1, maskdest1, maskdest2, duration));
                Coroutines.Start(Slide2D(mask2, maskdest2, maskdest1, duration));
                Coroutines.Start(Slide2D(whiteBackground1, whiteBackgroundDest1, whiteBackgroundDest2, duration));
                Coroutines.Start(Slide2D(whiteBackground2, whiteBackgroundDest2, whiteBackgroundDest1, duration));
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}