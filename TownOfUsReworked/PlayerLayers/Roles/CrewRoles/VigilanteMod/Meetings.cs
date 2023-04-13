using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.VigilanteMod
{
    public static class Meetings
    {
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.StartMeeting))]
        public static class StartMeetingPatch
        {
            public static void Prefix(PlayerControl __instance)
            {
                if (__instance == null)
                    return;

                foreach (var vigi in Role.GetRoles<Vigilante>(RoleEnum.Vigilante))
                {
                    if (vigi.PreMeetingDie)
                        Utils.RpcMurderPlayer(vigi.Player, vigi.Player);
                }
            }
        }

        [HarmonyPatch(typeof(ExileController), nameof(ExileController.Begin))]
        public static class MeetingExiledEnd
        {
            public static void Postfix()
            {
                foreach (var vigi in Role.GetRoles<Vigilante>(RoleEnum.Vigilante))
                {
                    if (vigi.PostMeetingDie)
                    {
                        vigi.Player.Exiled();
                        vigi.DeathReason = DeathReasonEnum.Suicide;
                    }
                }
            }
        }
    }
}