namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class TimeKeeper : Syndicate
    {
        public DateTime LastTimed;
        public CustomButton TimeButton;
        public bool Enabled;
        public float TimeRemaining;
        public bool Controlling => TimeRemaining > 0f;

        public override Color32 Color => ClientGameOptions.CustomSynColors ? Colors.TimeKeeper : Colors.Syndicate;
        public override string Name => "Time Keeper";
        public override LayerEnum Type => LayerEnum.TimeKeeper;
        public override RoleEnum RoleType => RoleEnum.TimeKeeper;
        public override Func<string> StartText => () => "Bend Time To Your Will";
        public override Func<string> AbilitiesText => () => $"- You can {(HoldsDrive ? "rewind players" : "freeze time, making people unable to move")}\n{CommonAbilities}";
        public override InspectorResults InspectorResults => InspectorResults.MovesAround;

        public TimeKeeper(PlayerControl player) : base(player)
        {
            RoleAlignment = RoleAlignment.SyndicatePower;
            TimeButton = new(this, "Time", AbilityTypes.Effect, "Secondary", TimeControl);
        }

        public float TimeTimer()
        {
            var timespan = DateTime.UtcNow - LastTimed;
            var num = Player.GetModifiedCooldown(CustomGameOptions.TimeControlCooldown) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public void Control()
        {
            if (!Enabled)
                Flash(Color, CustomGameOptions.TimeControlDuration);

            Enabled = true;
            TimeRemaining -= Time.deltaTime;

            if (HoldsDrive)
                CustomPlayer.AllPlayers.ForEach(x => GetRole(x).Rewinding = true);

            if (Meeting)
                TimeRemaining = 0f;
        }

        public void UnControl()
        {
            Enabled = false;
            LastTimed = DateTime.UtcNow;
            CustomPlayer.AllPlayers.ForEach(x => GetRole(x).Rewinding = false);
        }

        public void TimeControl()
        {
            TimeRemaining = CustomGameOptions.TimeControlDuration;
            Control();
            CallRpc(CustomRPC.Action, ActionsRPC.TimeControl, this);
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            TimeButton.Update(HoldsDrive ? "REWIND" : "FREEZE", TimeTimer(), CustomGameOptions.TimeControlCooldown, Controlling, TimeRemaining, CustomGameOptions.TimeControlDuration);
        }
    }
}