using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.VeteranMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDAlert
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Veteran))
                return;

            var role = Role.GetRole<Veteran>(PlayerControl.LocalPlayer);

            if (role.AlertButton == null)
                role.AlertButton = Utils.InstantiateButton();

            role.AlertButton.UpdateButton(role, "ALERT", role.AlertTimer(), CustomGameOptions.AlertCd, AssetManager.Alert, AbilityTypes.Effect, "ActionSecondary", null, role.ButtonUsable,
                role.ButtonUsable && !role.OnAlert, role.OnAlert, role.TimeRemaining, CustomGameOptions.AlertDuration, true, role.UsesLeft);
        }
    }
}