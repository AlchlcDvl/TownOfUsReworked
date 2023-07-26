namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Survivor : Neutral
    {
        public bool Enabled;
        public DateTime LastVested;
        public float TimeRemaining;
        public int UsesLeft;
        public bool ButtonUsable => UsesLeft > 0;
        public bool Vesting => TimeRemaining > 0f;
        public bool Alive => !Disconnected && !IsDead;
        public CustomButton VestButton;

        public override Color32 Color => ClientGameOptions.CustomNeutColors ? Colors.Survivor : Colors.Neutral;
        public override string Name => "Survivor";
        public override LayerEnum Type => LayerEnum.Survivor;
        public override RoleEnum RoleType => RoleEnum.Survivor;
        public override Func<string> StartText => () => "Do Whatever It Takes To Live";
        public override Func<string> AbilitiesText => () => "- You can put on a vest, which makes you unkillable for a short duration of time";
        public override InspectorResults InspectorResults => InspectorResults.LeadsTheGroup;

        public Survivor(PlayerControl player) : base(player)
        {
            UsesLeft = CustomGameOptions.MaxVests;
            RoleAlignment = RoleAlignment.NeutralBen;
            Objectives = () => "- Live to the end of the game";
            VestButton = new(this, "Vest", AbilityTypes.Effect, "ActionSecondary", HitVest, true);
        }

        public float VestTimer()
        {
            var timespan = DateTime.UtcNow - LastVested;
            var num = Player.GetModifiedCooldown(CustomGameOptions.VestCd) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public void Vest()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;

            if (Meeting)
                TimeRemaining = 0f;
        }

        public void UnVest()
        {
            Enabled = false;
            LastVested = DateTime.UtcNow;
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            VestButton.Update("PROTECT", VestTimer(), CustomGameOptions.VestCd, UsesLeft, Vesting, TimeRemaining, CustomGameOptions.VestDuration);
        }

        public void HitVest()
        {
            if (!ButtonUsable || VestTimer() != 0f || Vesting)
                return;

            TimeRemaining = CustomGameOptions.VestDuration;
            UsesLeft--;
            Vest();
            CallRpc(CustomRPC.Action, ActionsRPC.Vest, this);
        }
    }
}