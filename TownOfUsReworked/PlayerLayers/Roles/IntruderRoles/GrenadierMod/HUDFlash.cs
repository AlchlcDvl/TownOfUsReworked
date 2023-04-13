using HarmonyLib;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using System.Linq;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.GrenadierMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDFlash
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Grenadier))
                return;

            var role = Role.GetRole<Grenadier>(PlayerControl.LocalPlayer);

            if (role.FlashButton == null)
                role.FlashButton = CustomButtons.InstantiateButton();

            var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();
            var dummyActive = (bool)system?.dummy.IsActive;
            var sabActive = (bool)system?.specials.ToArray().Any(s => s.IsActive);
            role.FlashButton.UpdateButton(role, "FLASH", role.FlashTimer(), CustomGameOptions.GrenadeCd, AssetManager.Flash, AbilityTypes.Effect, "Secondary", null, true, !sabActive &&
                !dummyActive && !role.Flashed, role.Flashed, role.TimeRemaining, CustomGameOptions.GrenadeDuration);
        }
    }
}