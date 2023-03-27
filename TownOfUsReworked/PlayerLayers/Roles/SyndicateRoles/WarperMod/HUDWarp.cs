using HarmonyLib;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Modules;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.WarperMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDWarp
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Warper))
                return;

            var role = Role.GetRole<Warper>(PlayerControl.LocalPlayer);

            if (role.WarpButton == null)
                role.WarpButton = CustomButtons.InstantiateButton();

            role.WarpButton.UpdateButton(role, "WARP", role.WarpTimer(), CustomGameOptions.WarpCooldown, AssetManager.Warp, AbilityTypes.Effect, "Secondary");
        }
    }
}