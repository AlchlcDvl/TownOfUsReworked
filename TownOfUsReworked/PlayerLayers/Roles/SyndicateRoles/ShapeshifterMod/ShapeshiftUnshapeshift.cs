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
            if (ConstantVariables.IsLobby || ConstantVariables.IsEnded)
                return;

            foreach (var ss in Role.GetRoles<Shapeshifter>(RoleEnum.Shapeshifter))
            {
                if (ss.Shapeshifted)
                    ss.Shapeshift();
                else if (ss.Enabled)
                    ss.UnShapeshift();
            }
        }
    }
}