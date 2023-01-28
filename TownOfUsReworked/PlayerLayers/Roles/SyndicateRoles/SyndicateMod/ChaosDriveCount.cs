using HarmonyLib;
using TownOfUsReworked.Lobby.CustomOption;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.SyndicateMod
{
    public class ChaosDriveCount
    {
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.StartMeeting))]
        public class Sendchat
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                Role.ChaosDriveMeetingTimerCount += 1;

                if (Role.ChaosDriveMeetingTimerCount >= CustomGameOptions.ChaosDriveMeetingCount && !Role.SyndicateHasChaosDrive)
                    Role.SyndicateHasChaosDrive = true;
            }
        }
    }
}