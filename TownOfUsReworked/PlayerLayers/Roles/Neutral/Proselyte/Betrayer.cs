namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Betrayer : Neutral
    {
        public CustomButton KillButton { get; set; }
        public DateTime LastKilled { get; set; }

        public override Color32 Color => ClientGameOptions.CustomNeutColors ? Colors.Betrayer : Colors.Neutral;
        public override string Name => "Betrayer";
        public override LayerEnum Type => LayerEnum.Betrayer;
        public override RoleEnum RoleType => RoleEnum.Betrayer;
        public override Func<string> StartText => () => "Those Backs Are Ripe For Some Stabbing";
        public override Func<string> Description => () => "- You can kill";
        public override InspectorResults InspectorResults => InspectorResults.IsAggressive;
        public float Timer => ButtonUtils.Timer(Player, LastKilled, CustomGameOptions.BetrayerKillCooldown);

        public Betrayer(PlayerControl player) : base(player)
        {
            Objectives = () => $"- Kill anyone who opposes the {FactionName}";
            RoleAlignment = RoleAlignment.NeutralPros;
            KillButton = new(this, "BetKill", AbilityTypes.Direct, "ActionSecondary", Kill, Exception);
        }

        public void Kill()
        {
            if (IsTooFar(Player, KillButton.TargetPlayer) || Timer != 0f || Faction == Faction.Neutral)
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
            KillButton.Update("KILL", Timer, CustomGameOptions.BetrayerKillCooldown);
        }
    }
}