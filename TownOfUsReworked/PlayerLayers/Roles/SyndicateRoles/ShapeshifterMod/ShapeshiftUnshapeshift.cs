using HarmonyLib;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.ShapeshifterMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public static class ShapeshiftUnshapeshift
    {
        public static void Postfix()
        {
            foreach (var role in Role.GetRoles(RoleEnum.Shapeshifter))
            {
                var ss = (Shapeshifter)role;

                if (ss.Shapeshifted)
                    ss.Shapeshift();
                else if (ss.Enabled)
                    ss.UnShapeshift();
            }
        }
    }
}