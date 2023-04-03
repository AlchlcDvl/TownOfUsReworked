using System.Linq;
using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.EngineerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDFix
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Engineer))
                return;

            var role = Role.GetRole<Engineer>(PlayerControl.LocalPlayer);

            if (role.FixButton == null)
                role.FixButton = CustomButtons.InstantiateButton();

            var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();
            var dummyActive = (bool)system?.dummy.IsActive;
            var active = (bool)system?.specials.ToArray().Any(s => s.IsActive);
            role.FixButton.UpdateButton(role, "FIX", role.FixTimer(), CustomGameOptions.FixCooldown, AssetManager.Fix, AbilityTypes.Effect, "ActionSecondary", null, true, role.UsesLeft,
                role.ButtonUsable, active && !dummyActive);
        }
    }
}