using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.BansheeMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDScream
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Banshee))
                return;

            var role = Role.GetRole<Banshee>(PlayerControl.LocalPlayer);

            if (role.ScreamButton == null)
                role.ScreamButton = Utils.InstantiateButton();

            role.ScreamButton.UpdateButton(role, "SCREAM", role.ScreamTimer(), CustomGameOptions.ScreamCooldown, AssetManager.Placeholder, AbilityTypes.Effect, "Secondary",
                null, true, !role.Screaming, role.Screaming, role.TimeRemaining, CustomGameOptions.ScreamDuration, false, 0, true);
        }
    }
}