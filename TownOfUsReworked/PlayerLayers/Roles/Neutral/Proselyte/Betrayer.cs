namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Betrayer : Neutral
    {
        public CustomButton KillButton;
        public DateTime LastKilled;

        public override Color32 Color => ClientGameOptions.CustomNeutColors ? Colors.Betrayer : Colors.Neutral;
        public override string Name => "Betrayer";
        public override LayerEnum Type => LayerEnum.Betrayer;
        public override RoleEnum RoleType => RoleEnum.Betrayer;
        public override Func<string> StartText => () => "Those Backs Are Ripe For Some Stabbing";
        public override Func<string> AbilitiesText => () => "- You can kill";
        public override InspectorResults InspectorResults => InspectorResults.IsAggressive;

        public Betrayer(PlayerControl player) : base(player)
        {
            Objectives = () => $"- Kill anyone who opposes the {FactionName}";
            RoleAlignment = RoleAlignment.NeutralPros;
            KillButton = new(this, "BetKill", AbilityTypes.Direct, "ActionSecondary", Kill, Exception);
        }

        public float KillTimer()
        {
            var timespan = DateTime.UtcNow - LastKilled;
            var num = Player.GetModifiedCooldown(CustomGameOptions.BetrayerKillCooldown) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public void Kill()
        {
            if (IsTooFar(Player, KillButton.TargetPlayer) || KillTimer() != 0f || Faction == Faction.Neutral)
                return;

            var interact = Interact(Player, KillButton.TargetPlayer, true);

            if (interact[3] || interact[0])
                LastKilled = DateTime.UtcNow;
            else if (interact[1])
                LastKilled.AddSeconds(CustomGameOptions.ProtectKCReset);
            else if (interact[2])
                LastKilled.AddSeconds(CustomGameOptions.VestKCReset);
        }

        public bool Exception(PlayerControl player) => (player.Is(SubFaction) && SubFaction != SubFaction.None) || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate)
            || Player.IsLinkedTo(player);

        public override void UpdateHud(HudManager __instance)
        {
            if (Faction == Faction.Neutral)
                return;

            base.UpdateHud(__instance);
            KillButton.Update("KILL", KillTimer(), CustomGameOptions.BetrayerKillCooldown);
        }
    }
}