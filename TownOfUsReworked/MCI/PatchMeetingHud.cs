using HarmonyLib;

namespace TownOfUsReworked.MCI
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Confirm))]
    class SameVoteAll
    {
        public static void Postfix(MeetingHud __instance, ref byte suspectStateIdx)
        {
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                __instance.CmdCastVote(player.PlayerId, suspectStateIdx);
            }
        }
    }
}
