using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;

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

                foreach (var vigi in Role.GetRoles(RoleEnum.Vigilante))
                {
                    var role = (Vigilante)vigi;

                    if (role.PreMeetingDie)
                        Utils.RpcMurderPlayer(role.Player, role.Player);
                }
            }
        }

        [HarmonyPatch(typeof(ExileController), nameof(ExileController.Begin))]
        public static class MeetingExiledEnd
        {
            public static void Postfix()
            {
                foreach (var vigi in Role.GetRoles(RoleEnum.Vigilante))
                {
                    var role = (Vigilante)vigi;

                    if (role.PostMeetingDie)
                        role.Player.Exiled();
                }
            }
        }
    }
}