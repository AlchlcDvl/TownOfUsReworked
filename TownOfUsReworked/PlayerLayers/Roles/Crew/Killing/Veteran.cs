namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Veteran : CrewRole
    {
        public bool Enabled;
        public DateTime LastAlerted;
        public float TimeRemaining;
        public int UsesLeft;
        public bool ButtonUsable => UsesLeft > 0;
        public bool OnAlert => TimeRemaining > 0f;
        public CustomButton AlertButton;

        public Veteran(PlayerControl player) : base(player)
        {
            Name = "Veteran";
            StartText = "Alert To Kill Anyone Who Touches You";
            AbilitiesText = "- You can go on alert\n- When on alert, you will kill whoever interacts with you";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Veteran : Colors.Crew;
            RoleType = RoleEnum.Veteran;
            UsesLeft = CustomGameOptions.MaxAlerts;
            RoleAlignment = RoleAlignment.CrewKill;
            AlignmentName = CK;
            InspectorResults = InspectorResults.IsCold;
            Type = LayerEnum.Veteran;
            AlertButton = new(this, "Alert", AbilityTypes.Effect, "ActionSecondary", HitAlert, true);
        }

        public float AlertTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastAlerted;
            var num = Player.GetModifiedCooldown(CustomGameOptions.AlertCd) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Alert()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;

            if (MeetingHud.Instance)
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
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.Alert);
            writer.Write(PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            AlertButton.Update("ALERT", AlertTimer(), CustomGameOptions.AlertCd, UsesLeft, OnAlert, TimeRemaining, CustomGameOptions.AlertDuration, ButtonUsable, ButtonUsable);
        }
    }
}