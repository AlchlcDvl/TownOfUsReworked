namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Engineer : Crew
    {
        public CustomButton FixButton;
        public int UsesLeft;
        public bool ButtonUsable => UsesLeft > 0;
        public DateTime LastFixed;

        public override Color32 Color => ClientGameOptions.CustomCrewColors ? Colors.Engineer : Colors.Crew;
        public override string Name => "Engineer";
        public override LayerEnum Type => LayerEnum.Engineer;
        public override RoleEnum RoleType => RoleEnum.Engineer;
        public override Func<string> StartText => () => "Just Fix It";
        public override Func<string> AbilitiesText => () => "- You can fix sabotages at any time from anywhere\n- You can vent";
        public override InspectorResults InspectorResults => InspectorResults.NewLens;

        public Engineer(PlayerControl player) : base(player)
        {
            RoleAlignment = RoleAlignment.CrewSupport;
            UsesLeft = CustomGameOptions.MaxFixes;
            FixButton = new(this, "Fix", AbilityTypes.Effect, "ActionSecondary", Fix, true);
        }

        public float FixTimer()
        {
            var timespan = DateTime.UtcNow - LastFixed;
            var num = Player.GetModifiedCooldown(CustomGameOptions.FixCooldown) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public void Fix()
        {
            if (!ButtonUsable || FixTimer() != 0f)
                return;

            var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();

            if (system == null)
                return;

            var dummyActive = system.dummy.IsActive;
            var sabActive = system.specials.Any(s => s.IsActive);

            if (!sabActive || dummyActive)
                return;

            UsesLeft--;
            LastFixed = DateTime.UtcNow;
            FixExtentions.Fix();
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();
            var dummyActive = system?.dummy.IsActive;
            var active = system?.specials.Any(s => s.IsActive);
            var condition = active == true && dummyActive == false;
            FixButton.Update("FIX", FixTimer(), CustomGameOptions.FixCooldown, UsesLeft, condition && ButtonUsable, ButtonUsable);
        }
    }
}