using HarmonyLib;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.MultiClientInstancing
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Confirm))]
    [HarmonyPriority(Priority.Last)]
    public sealed class SameVoteAll
    {
        public static void Postfix(MeetingHud __instance, ref byte suspectStateIdx)
        {
            if (!ConstantVariables.IsLocalGame || !TownOfUsReworked.MCIActive)
                return;

            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                __instance.CmdCastVote(player.PlayerId, suspectStateIdx);
        }
    }
}