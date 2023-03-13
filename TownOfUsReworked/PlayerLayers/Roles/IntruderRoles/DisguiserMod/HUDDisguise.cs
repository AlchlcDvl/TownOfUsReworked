using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using System.Linq;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.DisguiserMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDDisguise
    {
        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Disguiser))
                return;

            var role = Role.GetRole<Disguiser>(PlayerControl.LocalPlayer);

            if (role.DisguiseButton == null)
                role.DisguiseButton = Utils.InstantiateButton();

            var targets = PlayerControl.AllPlayerControls.ToArray().ToList();

            if (CustomGameOptions.DisguiseTarget == DisguiserTargets.Intruders)
                targets = targets.Where(x => x.Is(Faction.Intruder)).ToList();
            else if (CustomGameOptions.DisguiseTarget == DisguiserTargets.NonIntruders)
                targets = targets.Where(x => !x.Is(Faction.Intruder)).ToList();
            
            if (role.MeasuredPlayer != null && targets.Contains(role.MeasuredPlayer))
                targets.Remove(role.MeasuredPlayer);

            role.DisguiseButton.UpdateButton(role, "DISGUISE", role.DisguiseTimer(), CustomGameOptions.DisguiseCooldown, TownOfUsReworked.DisguiseSprite, AbilityTypes.Direct,
                targets, role.MeasuredPlayer != null, true, role.DelayActive || role.Disguised, role.DelayActive ? role.TimeRemaining2 : role.TimeRemaining, role.DelayActive ?
                CustomGameOptions.TimeToDisguise : CustomGameOptions.DisguiseDuration);

            if (role.MeasureButton == null)
                role.MeasureButton = Utils.InstantiateButton();

            var notMeasured = PlayerControl.AllPlayerControls.ToArray().Where(x => role.MeasuredPlayer != x).ToList();
            role.MeasureButton.UpdateButton(role, "MEASURE", role.MeasureTimer(), CustomGameOptions.MeasureCooldown, TownOfUsReworked.MeasureSprite, AbilityTypes.Direct, 
                notMeasured);
        }
    }
}
