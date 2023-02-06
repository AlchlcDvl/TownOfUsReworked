using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.VigilanteMod
{
    public class Meetings
    {
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.StartMeeting))]
        public class StartMeetingPatch
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
                    
                    return;
                }
            }
        }

        [HarmonyPatch(typeof(ExileController), nameof(ExileController.Begin))]
        public class MeetingExiledEnd
        {
            private static void Postfix()
            {
                foreach (var vigi in Role.GetRoles(RoleEnum.Vigilante))
                {
                    var role = (Vigilante)vigi;

                    if (role.PostMeetingDie)
                        role.Player.Exiled();
                    
                    return;
                }
            }
        }
    }
}