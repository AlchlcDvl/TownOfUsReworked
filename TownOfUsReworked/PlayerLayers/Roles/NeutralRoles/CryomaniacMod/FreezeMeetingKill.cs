using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.CryomaniacMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.StartMeeting))]
    public class StartMeetingPatch
    {
        public static void Prefix(PlayerControl __instance, [HarmonyArgument(0)] GameData.PlayerInfo meetingTarget)
        {
            if (__instance == null)
                return;

            foreach (var cryo in Role.GetRoles(RoleEnum.Cryomaniac))
            {
                var role = (Cryomaniac)cryo;

                if (role.FreezeUsed)
                {
                    foreach (var player in role.DousedPlayers)
                    {
                        var player2 = Utils.PlayerById(player);
                        Utils.RpcMurderPlayer(role.Player, player2, false);
                    }

                    role.DousedPlayers.Clear();
                    role.FreezeUsed = false;
                }
            }
        }
    }
}