using TownOfUsReworked.Data;
using TownOfUsReworked.CustomOptions;
using System;
using TownOfUsReworked.Custom;
using TownOfUsReworked.Functions;
using System.Linq;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Engineer : CrewRole
    {
        public CustomButton FixButton;
        public int UsesLeft;
        public bool ButtonUsable => UsesLeft > 0;
        public DateTime LastFixed;

        public Engineer(PlayerControl player) : base(player)
        {
            Name = "Engineer";
            StartText = "Just Fix It";
            AbilitiesText = "- You can fix sabotages at any time during the game\n- You can vent";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Engineer : Colors.Crew;
            RoleType = RoleEnum.Engineer;
            RoleAlignment = RoleAlignment.CrewSupport;
            AlignmentName = CS;
            InspectorResults = InspectorResults.DifferentLens;
            UsesLeft = CustomGameOptions.MaxFixes;
            Type = LayerEnum.Engineer;
            FixButton = new(this, "Fix", AbilityTypes.Effect, "ActionSecondary", Fix, true);
        }

        public float FixTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastFixed;
            var num = Player.GetModifiedCooldown(CustomGameOptions.FixCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Fix()
        {
            if (!ButtonUsable || FixTimer() != 0f)
                return;

            var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();

            if (system == null)
                return;

            var dummyActive = system.dummy.IsActive;
            var sabActive = system.specials.ToArray().Any(s => s.IsActive);

            if (!sabActive || dummyActive)
                return;

            UsesLeft--;
            LastFixed = DateTime.UtcNow;
            FixFunctions.Fix();
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();
            var dummyActive = system?.dummy.IsActive;
            var active = system?.specials.ToArray().Any(s => s.IsActive);
            var condition = active == true && dummyActive == false;
            FixButton.Update("FIX", FixTimer(), CustomGameOptions.FixCooldown, UsesLeft, condition && ButtonUsable, ButtonUsable);
        }
    }
}