using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.PoisonerMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.StartMeeting))]
    public class StartMeetingPatch
    {
        public static void Prefix(PlayerControl __instance, [HarmonyArgument(0)] GameData.PlayerInfo meetingTarget)
        {
            if (__instance == null)
                return;

            foreach (var role in Role.GetRoles(RoleEnum.Poisoner))
            {
               var poisoner = (Poisoner)role;

                if (poisoner.Player != poisoner.PoisonedPlayer && poisoner.PoisonedPlayer != null)
                {
                    if (!poisoner.PoisonedPlayer.Data.IsDead && !poisoner.PoisonedPlayer.Is(RoleEnum.Pestilence))
                        Utils.MurderPlayer(poisoner.Player, poisoner.PoisonedPlayer);
                }
            }
        }
    }
}