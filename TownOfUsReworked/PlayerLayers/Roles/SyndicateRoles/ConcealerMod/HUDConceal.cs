using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Modules;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.ConcealerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDConceal
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Concealer))
                return;

            var role = Role.GetRole<Concealer>(PlayerControl.LocalPlayer);

            if (role.ConcealButton == null)
                role.ConcealButton = CustomButtons.InstantiateButton();

            role.ConcealButton.UpdateButton(role, "CONCEAL", role.ConcealTimer(), CustomGameOptions.ConcealCooldown, AssetManager.Placeholder, AbilityTypes.Effect, "Secondary",
                null, true, !role.Concealed, role.Concealed, role.TimeRemaining, CustomGameOptions.ConcealDuration);
        }
    }
}