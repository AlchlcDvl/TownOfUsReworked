using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.ConsortMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class BlockUnBlock
    {
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Consort))
            {
                var cons = (Consort)role;

                if (cons.Blocking)
                    cons.Block();
                else if (cons.Enabled)
                    cons.UnBlock();
            }
        }
    }
}