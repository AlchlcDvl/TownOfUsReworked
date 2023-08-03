namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class TimeKeeper : Syndicate
    {
        public DateTime LastTimed { get; set; }
        public CustomButton TimeButton { get; set; }
        public bool Enabled { get; set; }
        public float TimeRemaining { get; set; }
        public bool Controlling => TimeRemaining > 0f;

        public override Color32 Color => ClientGameOptions.CustomSynColors ? Colors.TimeKeeper : Colors.Syndicate;
        public override string Name => "Time Keeper";
        public override LayerEnum Type => LayerEnum.TimeKeeper;
        public override RoleEnum RoleType => RoleEnum.TimeKeeper;
        public override Func<string> StartText => () => "Bend Time To Your Will";
        public override Func<string> Description => () => $"- You can {(HoldsDrive ? "rewind players" : "freeze time, making people unable to move")}\n{CommonAbilities}";
        public override InspectorResults InspectorResults => InspectorResults.MovesAround;
        public float Timer => ButtonUtils.Timer(Player, LastTimed, CustomGameOptions.TimeControlCooldown);

        public TimeKeeper(PlayerControl player) : base(player)
        {
            RoleAlignment = RoleAlignment.SyndicatePower;
            TimeButton = new(this, "Time", AbilityTypes.Effect, "Secondary", TimeControl);
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
            if (Timer != 0f || Controlling)
                return;

            TimeRemaining = CustomGameOptions.TimeControlDuration;
            Control();
            CallRpc(CustomRPC.Action, ActionsRPC.TimeControl, this);
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            TimeButton.Update(HoldsDrive ? "REWIND" : "FREEZE", Timer, CustomGameOptions.TimeControlCooldown, Controlling, TimeRemaining, CustomGameOptions.TimeControlDuration);
        }
    }
}