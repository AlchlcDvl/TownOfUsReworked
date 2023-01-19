using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.EscortMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class BlockUnBlock
    {
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Escort))
            {
                var esc = (Escort)role;

                if (esc.Blocking)
                    esc.Block();
                else if (esc.Enabled)
                    esc.UnBlock();
            }
        }
    }
}