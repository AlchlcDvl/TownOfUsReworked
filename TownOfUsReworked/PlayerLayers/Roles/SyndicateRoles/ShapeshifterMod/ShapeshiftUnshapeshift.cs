using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.ShapeshifterMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class ShapeshiftUnshapeshift
    {
        public static bool ShapeshiftEnabled;

        public static void Postfix(HudManager __instance)
        {
            ShapeshiftEnabled = false;

            foreach (var role in Role.GetRoles(RoleEnum.Shapeshifter))
            {
                var ss = (Shapeshifter) role;

                if (ss.Shapeshifted)
                {
                    ShapeshiftEnabled = true;
                    ss.Shapeshift();
                }
                else if (ss.Enabled)
                {
                    ShapeshiftEnabled = false;
                    ss.UnShapeshift();
                }
            }
        }
    }
}