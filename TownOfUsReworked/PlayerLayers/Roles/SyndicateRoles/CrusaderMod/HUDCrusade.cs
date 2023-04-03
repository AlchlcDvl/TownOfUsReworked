using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.CrusaderMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDCrusade
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Crusader))
                return;

            var role = Role.GetRole<Crusader>(PlayerControl.LocalPlayer);

            if (role.CrusadeButton == null)
                role.CrusadeButton = CustomButtons.InstantiateButton();

            role.CrusadeButton.UpdateButton(role, "CRUSADE", role.CrusadeTimer(), CustomGameOptions.CrusadeCooldown, AssetManager.Placeholder, AbilityTypes.Direct, "Secondary",
                role.OnCrusade, role.TimeRemaining, CustomGameOptions.CrusadeDuration, true, !role.OnCrusade);
        }
    }
}