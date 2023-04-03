using HarmonyLib;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using System.Linq;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.DisguiserMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDDisguise
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Disguiser))
                return;

            var role = Role.GetRole<Disguiser>(PlayerControl.LocalPlayer);

            if (role.DisguiseButton == null)
                role.DisguiseButton = CustomButtons.InstantiateButton();

            var targets = PlayerControl.AllPlayerControls.ToArray().Where(x => (x.Is(Faction.Intruder) && CustomGameOptions.DisguiseTarget == DisguiserTargets.Intruders) ||
                (!x.Is(Faction.Intruder) && CustomGameOptions.DisguiseTarget == DisguiserTargets.NonIntruders) || CustomGameOptions.DisguiseTarget == DisguiserTargets.Everyone).ToList();

            if (role.MeasuredPlayer != null && targets.Contains(role.MeasuredPlayer))
                targets.Remove(role.MeasuredPlayer);

            role.DisguiseButton.UpdateButton(role, "DISGUISE", role.DisguiseTimer(), CustomGameOptions.DisguiseCooldown, AssetManager.Disguise, AbilityTypes.Direct, "Secondary",
                targets, role.MeasuredPlayer != null, true, role.DelayActive || role.Disguised, role.DelayActive ? role.TimeRemaining2 : role.TimeRemaining, role.DelayActive ?
                CustomGameOptions.TimeToDisguise : CustomGameOptions.DisguiseDuration);

            if (role.MeasureButton == null)
                role.MeasureButton = CustomButtons.InstantiateButton();

            var notMeasured = PlayerControl.AllPlayerControls.ToArray().Where(x => role.MeasuredPlayer != x).ToList();
            role.MeasureButton.UpdateButton(role, "MEASURE", role.MeasureTimer(), CustomGameOptions.MeasureCooldown, AssetManager.Measure, AbilityTypes.Direct, "Tertiary", notMeasured);
        }
    }
}
