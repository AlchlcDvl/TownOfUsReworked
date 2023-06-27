namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class TimeKeeper : Syndicate
    {
        public DateTime LastTimed;
        public CustomButton TimeButton;
        public bool Enabled;
        public float TimeRemaining;
        public bool Controlling => TimeRemaining > 0f;

        public TimeKeeper(PlayerControl player) : base(player)
        {
            Name = "Time Keeper";
            StartText = () => "Bend Time To Your Will";
            AbilitiesText = () => $"- You can freeze time, making people unable to move\n- With the Chaos Drive, you rewind players instead\n{CommonAbilities}";
            RoleType = RoleEnum.TimeKeeper;
            RoleAlignment = RoleAlignment.SyndicatePower;
            Color = CustomGameOptions.CustomSynColors ? Colors.TimeKeeper : Colors.Syndicate;
            Type = LayerEnum.TimeKeeper;
            TimeButton = new(this, "Time", AbilityTypes.Effect, "Secondary", TimeControl);
            InspectorResults = InspectorResults.MovesAround;

            if (TownOfUsReworked.IsTest)
                Utils.LogSomething($"{Player.name} is {Name}");
        }

        public float TimeTimer()
        {
            var timespan = DateTime.UtcNow - LastTimed;
            var num = Player.GetModifiedCooldown(CustomGameOptions.TimeControlCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Control()
        {
            if (!Enabled)
                Utils.Flash(Color, CustomGameOptions.TimeControlDuration);

            Enabled = true;
            TimeRemaining -= Time.deltaTime;

            if (HoldsDrive)
            {
                foreach (var player in CustomPlayer.AllPlayers)
                    GetRole(player).Rewinding = true;
            }

            if (Utils.Meeting)
                TimeRemaining = 0f;
        }

        public void UnControl()
        {
            Enabled = false;
            LastTimed = DateTime.UtcNow;

            foreach (var player in CustomPlayer.AllPlayers)
                GetRole(player).Rewinding = false;
        }

        public void TimeControl()
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.TimeControl);
            writer.Write(PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            TimeRemaining = CustomGameOptions.TimeControlDuration;
            Control();
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            TimeButton.Update(HoldsDrive ? "REWIND" : "FREEZE", TimeTimer(), CustomGameOptions.TimeControlCooldown, Controlling, TimeRemaining, CustomGameOptions.TimeControlDuration);
        }
    }
}