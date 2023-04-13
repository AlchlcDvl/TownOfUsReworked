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

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch]
    public static class MeetingPatches
    {
        #pragma warning disable
        private static GameData.PlayerInfo voteTarget = null;
        public static int MeetingCount;
        #pragma warning restore

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
        public static class CamoMeetings
        {
            public static void Postfix(MeetingHud __instance)
            {
                if (CustomGameOptions.MeetingColourblind && DoUndo.IsCamoed)
                {
                    foreach (var state in __instance.playerStates)
                    {
                        state.NameText.color = Color.clear;
                        state.PlayerIcon.SetBodyColor(6);
                        state.PlayerIcon.SetHat("None", 0);
                        state.PlayerIcon.SetSkin("None", 0);
                        state.PlayerIcon.SetName(" ");
                    }
                }
            }
        }

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
            public static void Postfix()
            {
                foreach (var player in PlayerControl.AllPlayerControls)
                    player.MyPhysics.ResetAnimState();

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
            public static void Prefix([HarmonyArgument(0)]GameData.PlayerInfo meetingTarget) => voteTarget = meetingTarget;
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
        public static class MeetingHudUpdatePatch
        {
            public static void Postfix(MeetingHud __instance)
            {
                //Deactivate skip Button if skipping on emergency meetings is disabled
                if ((voteTarget == null && CustomGameOptions.SkipButtonDisable == DisableSkipButtonMeetings.Emergency) || (CustomGameOptions.SkipButtonDisable ==
                    DisableSkipButtonMeetings.Always))
                {
                    __instance.SkipVoteButton.gameObject.SetActive(false);
                }
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
                    if (rebel.FormerRole?.RoleType != RoleEnum.Politician)
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
    }
}