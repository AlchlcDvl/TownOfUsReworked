using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using System.Linq;
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
                
            var cryos = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(RoleEnum.Cryomaniac)).ToList();

            foreach (var cryo in cryos)
            {
                var role = Role.GetRole<Cryomaniac>(cryo);

                if (role.FreezeUsed)
                {
                    foreach (var player in role.DousedPlayers)
                    {
                        var player2 = Utils.PlayerById(player);
                        Utils.MurderPlayer(cryo, player2);
                    }
                }
                
                return;
            }
        }
    }
}