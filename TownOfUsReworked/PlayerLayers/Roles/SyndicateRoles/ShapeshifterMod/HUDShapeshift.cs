using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.ShapeshifterMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDShapeshift
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Shapeshifter))
                return;

            var role = Role.GetRole<Shapeshifter>(PlayerControl.LocalPlayer);

            if (role.ShapeshiftButton == null)
                role.ShapeshiftButton = Utils.InstantiateButton();

            role.ShapeshiftButton.UpdateButton(role, "SHAPESHIFT", role.ShapeshiftTimer(), CustomGameOptions.ShapeshiftCooldown, AssetManager.Shapeshift, AbilityTypes.Effect, "Secondary",
                role.Shapeshifted, role.TimeRemaining, CustomGameOptions.ShapeshiftDuration);
        }
    }
}