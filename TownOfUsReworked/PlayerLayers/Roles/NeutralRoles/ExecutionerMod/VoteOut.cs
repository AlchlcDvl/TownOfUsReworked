using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.ExecutionerMod
{
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.Begin))]
    internal class MeetingExiledEnd
    {
        private static void Postfix(ExileController __instance)
        {
            var exiled = __instance.exiled;

            if (exiled == null)
                return;

            var player = exiled.Object;

            foreach (var role in Role.GetRoles(RoleEnum.Executioner))
            {
                if (player.PlayerId == ((Executioner)role).TargetPlayer.PlayerId)
                {
                    if (CustomGameOptions.JestEjectScreen)
                        __instance.completeString = "You feel a sense of dread during the ejection. The <color=#CCCCCCFF>Executioner</color> has won!";

                    ((Executioner)role).Wins();
                }
            }        
        }
    }
}