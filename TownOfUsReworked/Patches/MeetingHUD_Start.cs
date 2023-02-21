using HarmonyLib;
using Object = UnityEngine.Object;
using Hazel;
using Reactor.Utilities.Extensions;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.Patches
{
    class Meeting
    {
        private static GameData.PlayerInfo voteTarget = null;

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
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.RemoveAllBodies, SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(writer);

                var buggedBodies = Object.FindObjectsOfType<DeadBody>();

                foreach (var body in buggedBodies)
                    body.gameObject.Destroy();
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