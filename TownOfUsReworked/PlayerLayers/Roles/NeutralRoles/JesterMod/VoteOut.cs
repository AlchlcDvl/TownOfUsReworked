using HarmonyLib;
using TownOfUsReworked.Enums;
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
                __instance.completeString = "You feel a sense of dread during this ejection. The Jester has won!";
                ((Jester) role).Wins();
            }
        }
    }
}