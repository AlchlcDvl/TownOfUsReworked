using HarmonyLib;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.EnforcerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDPlace
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Enforcer))
                return;

            var role = Role.GetRole<Enforcer>(PlayerControl.LocalPlayer);

            if (role.BombButton == null)
                role.BombButton = CustomButtons.InstantiateButton();

            role.BombButton.UpdateButton(role, "PLANT", role.BombTimer(), CustomGameOptions.EnforceCooldown, AssetManager.Placeholder, AbilityTypes.Direct, "Secondary",
                null, true, true, role.DelayActive || role.Bombing, role.DelayActive ? role.TimeRemaining2 : role.TimeRemaining, role.DelayActive ? CustomGameOptions.EnforceDelay :
                CustomGameOptions.EnforceDuration);
        }
    }
}
