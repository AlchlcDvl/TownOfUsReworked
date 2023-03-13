using System.Linq;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.EngineerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDFix
    {
        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Engineer))
                return;

            var role = Role.GetRole<Engineer>(PlayerControl.LocalPlayer);

            if (role.FixButton == null)
                role.FixButton = Utils.InstantiateButton();

            var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();
            var dummyActive = (bool)system?.dummy.IsActive;
            var active = (bool)system?.specials.ToArray().Any(s => s.IsActive);
            role.FixButton.UpdateButton(role, "FIX", role.FixTimer(), CustomGameOptions.FixCooldown, TownOfUsReworked.EngineerFix, AbilityTypes.Effect, null, true,
                role.UsesLeft, role.ButtonUsable, active && !dummyActive);
        }
    }
}