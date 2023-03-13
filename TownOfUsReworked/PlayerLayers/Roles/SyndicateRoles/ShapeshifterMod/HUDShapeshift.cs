using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.ShapeshifterMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDShapeshift
    {
        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Shapeshifter))
                return;

            var role = Role.GetRole<Shapeshifter>(PlayerControl.LocalPlayer);

            if (role.ShapeshiftButton == null)
                role.ShapeshiftButton = Utils.InstantiateButton();

            role.ShapeshiftButton.UpdateButton(role, "SHAPESHIFT", role.ShapeshiftTimer(), CustomGameOptions.ShapeshiftCooldown, TownOfUsReworked.Shapeshift, AbilityTypes.Effect, 
                role.Shapeshifted, role.TimeRemaining, CustomGameOptions.ShapeshiftDuration);
        }
    }
}