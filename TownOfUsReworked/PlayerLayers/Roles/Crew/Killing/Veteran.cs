namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Veteran : Crew
    {
        public bool Enabled;
        public DateTime LastAlerted;
        public float TimeRemaining;
        public int UsesLeft;
        public bool ButtonUsable => UsesLeft > 0;
        public bool OnAlert => TimeRemaining > 0f;
        public CustomButton AlertButton;

        public override Color32 Color => ClientGameOptions.CustomCrewColors ? Colors.Veteran : Colors.Crew;
        public override string Name => "Veteran";
        public override LayerEnum Type => LayerEnum.Veteran;
        public override RoleEnum RoleType => RoleEnum.Veteran;
        public override Func<string> StartText => () => "Alert To Kill Anyone Who Dares To Touches You";
        public override Func<string> AbilitiesText => () => "- You can go on alert\n- When on alert, you will kill whoever interacts with you";
        public override InspectorResults InspectorResults => InspectorResults.IsCold;

        public Veteran(PlayerControl player) : base(player)
        {
            UsesLeft = CustomGameOptions.MaxAlerts;
            RoleAlignment = RoleAlignment.CrewKill;
            AlertButton = new(this, "Alert", AbilityTypes.Effect, "ActionSecondary", HitAlert, true);
        }

        public float AlertTimer()
        {
            var timespan = DateTime.UtcNow - LastAlerted;
            var num = Player.GetModifiedCooldown(CustomGameOptions.AlertCd) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public void Alert()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;

            if (Meeting)
                TimeRemaining = 0f;
        }

        public void UnAlert()
        {
            Enabled = false;
            LastAlerted = DateTime.UtcNow;
        }

        public void HitAlert()
        {
            if (!ButtonUsable || AlertTimer() != 0f || OnAlert)
                return;

            TimeRemaining = CustomGameOptions.AlertDuration;
            UsesLeft--;
            Alert();
            CallRpc(CustomRPC.Action, ActionsRPC.Alert, this);
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            AlertButton.Update("ALERT", AlertTimer(), CustomGameOptions.AlertCd, UsesLeft, OnAlert, TimeRemaining, CustomGameOptions.AlertDuration, ButtonUsable, ButtonUsable);
        }
    }
}