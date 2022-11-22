using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.JesterMod
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
            var role = Role.GetRole(player);

            if (role == null)
                return;

            if (role.RoleType == RoleEnum.Jester) 
            {
                if (CustomGameOptions.JestEjectScreen)
                    __instance.completeString = "You feel a sense of dread during this ejection. The <color=#F7B3DAFF>Jester</color> has won!";
                    
                ((Jester)role).Wins();
            }
        }
    }
}