using HarmonyLib;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.DrunkardMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDConfuse
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Drunkard))
                return;

            var role = Role.GetRole<Drunkard>(PlayerControl.LocalPlayer);

            if (role.ConfuseButton == null)
                role.ConfuseButton = CustomButtons.InstantiateButton();

            role.ConfuseButton.UpdateButton(role, "CONFUSE", role.DrunkTimer(), CustomGameOptions.ConfuseCooldown, AssetManager.Placeholder, AbilityTypes.Effect, "Secondary", role.Confused,
                role.TimeRemaining, CustomGameOptions.ConfuseDuration, true, !role.Confused);
        }
    }
}