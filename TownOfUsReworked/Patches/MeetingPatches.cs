using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.CamouflagerMod;
using Object = UnityEngine.Object;
using Hazel;
using Reactor.Utilities.Extensions;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.Patches
{
    class MeetingPatches
    {
        private static GameData.PlayerInfo voteTarget = null;

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
        public class CamoMeetings
        {
            private static void Postfix(MeetingHud __instance)
            {
                if (CustomGameOptions.MeetingColourblind && CamouflageUnCamouflage.IsCamoed)
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
        public class PlayerStates
        {
            private static void Postfix(PlayerVoteArea __instance, [HarmonyArgument(0)] GameData.PlayerInfo playerInfo)
            {
                if (CustomGameOptions.MeetingColourblind && CamouflageUnCamouflage.IsCamoed)
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
        public class PlayerPreviews
        {
            private static void Postfix(PlayerVoteArea __instance, [HarmonyArgument(0)] string plateId)
            {
                if (CustomGameOptions.MeetingColourblind && CamouflageUnCamouflage.IsCamoed)
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
        public class MeetingHUD_Start
        {
            public static void Postfix(MeetingHud __instance)
            {
                foreach (var player in PlayerControl.AllPlayerControls)
                    player.MyPhysics.ResetAnimState();
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Close))]
        public class MeetingHud_Close
        {
            public static void Postfix(MeetingHud __instance)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.RemoveAllBodies, SendOption.Reliable);
                AmongUsClient.Instance.FinishRpcImmediately(writer);

                var buggedBodies = Object.FindObjectsOfType<DeadBody>();

                foreach (var body in buggedBodies)
                    body.gameObject.Destroy();

                foreach (var player in PlayerControl.AllPlayerControls)
                    player.MyPhysics.ResetAnimState();
            }
        }
        
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.StartMeeting))]
        class StartMeetingPatch
        {
            public static void Prefix(PlayerControl __instance, [HarmonyArgument(0)]GameData.PlayerInfo meetingTarget)
            {
                voteTarget = meetingTarget;
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
        class MeetingHudUpdatePatch
        {
            static void Postfix(MeetingHud __instance)
            {
                //Deactivate skip Button if skipping on emergency meetings is disabled 
                if ((voteTarget == null && CustomGameOptions.SkipButtonDisable == DisableSkipButtonMeetings.Emergency) || (CustomGameOptions.SkipButtonDisable ==
                    DisableSkipButtonMeetings.Always))
                    __instance.SkipVoteButton.gameObject.SetActive(false);
            }
        }
    }
}
