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
using TownOfUsReworked.Custom;
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
                    __instance.Background.sprite = DestroyableSingleton<HatManager>.Instance.GetNamePlateById("nameplate_NoPlate").viewData.viewData.Image;
                    __instance.LevelNumberText.GetComponentInParent<SpriteRenderer>().enabled = false;
                    __instance.LevelNumberText.GetComponentInParent<SpriteRenderer>().gameObject.SetActive(false);
                    __instance.NameText.color = Palette.White;
                }
                else
                {
                    if (CustomGameOptions.WhiteNameplates)
                        __instance.Background.sprite = DestroyableSingleton<HatManager>.Instance.GetNamePlateById("nameplate_NoPlate").viewData.viewData.Image;

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
            public static void Postfix(RoomTracker __instance)
            {
                if (__instance.text.transform.localPosition.y != -3.25f)
                    Location = __instance.text.text;
                else
                {
                    var name = PlayerControl.LocalPlayer.name;
                    Location = $"a hallway or somewhere outside, {name} where is the body?";
                }
            }
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
                    __instance.Background.sprite = DestroyableSingleton<HatManager>.Instance.GetNamePlateById("nameplate_NoPlate").viewData.viewData.Image;
                    __instance.LevelNumberText.GetComponentInParent<SpriteRenderer>().enabled = false;
                    __instance.LevelNumberText.GetComponentInParent<SpriteRenderer>().gameObject.SetActive(false);
                    __instance.NameText.color = Palette.White;
                }
                else
                {
                    if (CustomGameOptions.WhiteNameplates)
                        __instance.Background.sprite = DestroyableSingleton<HatManager>.Instance.GetNamePlateById("nameplate_NoPlate").viewData.viewData.Image;

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
                Coroutines.Start(Announcements(Reported, __instance));

                foreach (var player in PlayerControl.AllPlayerControls)
                    player?.MyPhysics?.ResetAnimState();

                MeetingCount++;

                foreach (var role in Role.AllRoles)
                    role.Zooming = false;

                if (Role.ChaosDriveMeetingTimerCount < CustomGameOptions.ChaosDriveMeetingCount)
                    Role.ChaosDriveMeetingTimerCount++;

                if (Role.ChaosDriveMeetingTimerCount == CustomGameOptions.ChaosDriveMeetingCount && !Role.SyndicateHasChaosDrive)
                {
                    Role.SyndicateHasChaosDrive = true;
                    RoleGen.AssignChaosDrive();
                }

                foreach (var layer in PlayerLayer.AllLayers)
                    layer?.OnMeetingStart(__instance);
            }

            private static IEnumerator Announcements(GameData.PlayerInfo target, MeetingHud __instance)
            {
                foreach (var layer in PlayerLayer.AllLayers)
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
                        var killerRole = Role.GetRole(killer);

                        if (CustomGameOptions.KillerReports == RoleFactionReports.Role)
                            report = $"They were killed by {a_an} {killerRole.Name}.";
                        else if (CustomGameOptions.KillerReports == RoleFactionReports.Faction)
                            report = $"They were killed by a member of the {killerRole.FactionName}.";
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
                    message = "The Syndicate possesses the Chaod Drive.";

                HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, message);
                var writer5 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SendChat, SendOption.Reliable);
                writer5.Write(message);
                AmongUsClient.Instance.FinishRpcImmediately(writer5);

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
            public static void Postfix()
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.MeetingStart, SendOption.Reliable);
                AmongUsClient.Instance.FinishRpcImmediately(writer);

                foreach (var body in Object.FindObjectsOfType<DeadBody>())
                    body.gameObject.Destroy();

                foreach (var player in PlayerControl.AllPlayerControls)
                    player.MyPhysics.ResetAnimState();
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
                if ((VoteTarget == null && CustomGameOptions.SkipButtonDisable == DisableSkipButtonMeetings.Emergency) || (CustomGameOptions.SkipButtonDisable ==
                    DisableSkipButtonMeetings.Always))
                {
                    __instance.SkipVoteButton.gameObject.SetActive(false);
                }

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

                foreach (var button in CustomButton.AllButtons)
                    button?.Disable();
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.VotingComplete))]
        public static class VotingComplete
        {
            public static void Postfix([HarmonyArgument(1)] GameData.PlayerInfo exiled, [HarmonyArgument(2)] bool tie)
            {
                var exiledString = exiled == null ? "null" : exiled.PlayerName;
                Utils.LogSomething($"Exiled PlayerName = {exiledString}");
                Utils.LogSomething($"Was a tie = {tie}");
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

                foreach (var politician in Role.GetRoles<Politician>(RoleEnum.Politician))
                {
                    var playerInfo = politician.Player.Data;
                    var anonVotesOption = CustomGameOptions.AnonymousVoting;
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

                    GameOptionsManager.Instance.currentNormalGameOptions.AnonymousVotes = anonVotesOption;
                }

                foreach (var rebel in Role.GetRoles<PromotedRebel>(RoleEnum.PromotedRebel))
                {
                    if (!rebel.IsPol)
                        continue;

                    var playerInfo = rebel.Player.Data;
                    var anonVotesOption = CustomGameOptions.AnonymousVoting;
                    GameOptionsManager.Instance.currentNormalGameOptions.AnonymousVotes = CustomGameOptions.PoliticianAnonymous;

                    foreach (var extraVote in rebel.ExtraVotes)
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

                    GameOptionsManager.Instance.currentNormalGameOptions.AnonymousVotes = anonVotesOption;
                }

                foreach (var mayor in Role.GetRoles<Mayor>(RoleEnum.Mayor))
                {
                    var playerInfo = mayor.Player.Data;
                    var anonVotesOption = CustomGameOptions.AnonymousVoting;
                    GameOptionsManager.Instance.currentNormalGameOptions.AnonymousVotes = CustomGameOptions.MayorAnonymous;

                    foreach (var extraVote in mayor.ExtraVotes)
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

                    GameOptionsManager.Instance.currentNormalGameOptions.AnonymousVotes = anonVotesOption;
                }

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

                if (CustomGameOptions.AnonymousVoting && !(deadFlag || insiderFlag))
                    PlayerMaterial.SetColors(Palette.DisabledGrey, spriteRenderer);
                else
                    PlayerMaterial.SetColors(voterPlayer.DefaultOutfit.ColorId, spriteRenderer);

                spriteRenderer.transform.SetParent(parent);
                spriteRenderer.transform.localScale = Vector3.zero;
                var Base = __instance as MonoBehaviour;
                Base.StartCoroutine(Effects.Bloop(index * 0.3f, spriteRenderer.transform, 1f, 0.5f));
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

            if (player.CanDoTasks() && (PlayerControl.LocalPlayer.PlayerId == player.TargetPlayerId || (PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything)) &&
                info[0] != null)
            {
                var role = info[0] as Role;
                name += $"{((DoUndo.IsCamoed && CustomGameOptions.MeetingColourblind) || PlayerControl.LocalPlayer.Data.IsDead ? name : "")} ({role.TasksCompleted}/{role.TotalTasks})";
                roleRevealed = true;
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
                    if (CustomGameOptions.ExeKnowsTargetRole)
                    {
                        var role = info[0] as Role;
                        color = role.Color;
                        name += $"\n{role.Name}";
                        roleRevealed = true;
                    }
                    else
                        color = executioner.Color;

                    name += " <color=#CCCCCCFF>§</color>";
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
                    if (CustomGameOptions.GAKnowsTargetRole)
                    {
                        var role = info[0] as Role;
                        color = role.Color;
                        name += $"\n{role.Name}";
                        roleRevealed = true;
                    }
                    else
                        color = guardianAngel.Color;

                    name += " <color=#FFFFFFFF>★</color>";
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
                        name += $"\n{role.Name}";
                        name += " <color=#F995FCFF>Λ</color>";
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
                        name += $"\n{role.Name}";
                        name += " <color=#7B8968FF>γ</color>";
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
                        name += $"\n{role.Name}";
                        name += " <color=#575657FF>$</color>";
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
                        name += $"\n{role.Name}";
                        name += " <color=#E6108AFF>Σ</color>";
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
                        name += $"\n{role.Name}";
                        name += " <color=#7B8968FF>γ</color>";
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
                        name += $"\n{role.Name}";
                        name += " <color=#575657FF>$</color>";
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
                        name += $"\n{role.Name}";
                        name += " <color=#E6108AFF>Σ</color>";
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
                        name += $"\n{role.Name}";
                        name += " <color=#F995FCFF>Λ</color>";
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

                    name += $" {lover.ColoredSymbol}";
                }
            }
            else if (PlayerControl.LocalPlayer.Is(ObjectifierEnum.Rivals) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                var rival = localinfo[3] as Objectifier;
                var otherRival = PlayerControl.LocalPlayer.GetOtherRival();

                if (otherRival == player)
                {
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

                    name += $" {rival.ColoredSymbol}";
                }
            }
            else if (PlayerControl.LocalPlayer.Is(ObjectifierEnum.Mafia) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything))
            {
                var mafia = localinfo[3] as Mafia;

                if (player.Is(ObjectifierEnum.Mafia) && player.TargetPlayerId != PlayerControl.LocalPlayer.PlayerId)
                {
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

                    name += $" {mafia.ColoredSymbol}";
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
    }
}