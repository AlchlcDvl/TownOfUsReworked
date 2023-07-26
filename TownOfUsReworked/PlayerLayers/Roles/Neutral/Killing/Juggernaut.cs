namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Juggernaut : Neutral
    {
        public DateTime LastKilled;
        public int JuggKills;
        public CustomButton AssaultButton;

        public override Color32 Color => ClientGameOptions.CustomNeutColors ? Colors.Juggernaut : Colors.Neutral;
        public override string Name => "Juggernaut";
        public override LayerEnum Type => LayerEnum.Juggernaut;
        public override RoleEnum RoleType => RoleEnum.Juggernaut;
        public override Func<string> StartText => () => "Your Power Grows With Every Kill";
        public override Func<string> AbilitiesText => () => "- With each kill, your kill cooldown decreases" + (JuggKills >= 4 ? "\n- You can bypass all forms of protection" : "");
        public override InspectorResults InspectorResults => InspectorResults.IsAggressive;

        public Juggernaut(PlayerControl player) : base(player)
        {
            Objectives = () => "- Assault anyone who can oppose you";
            RoleAlignment = RoleAlignment.NeutralKill;
            JuggKills = 0;
            AssaultButton = new(this, "Assault", AbilityTypes.Direct, "ActionSecondary", Assault, Exception);
        }

        public float AssaultTimer()
        {
            var timespan = DateTime.UtcNow - LastKilled;
            var num = Player.GetModifiedCooldown(CustomGameOptions.JuggKillCooldown, -(CustomGameOptions.JuggKillBonus * JuggKills)) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public void Assault()
        {
            if (IsTooFar(Player, AssaultButton.TargetPlayer) || AssaultTimer() != 0f)
                return;

            var interact = Interact(Player, AssaultButton.TargetPlayer, true, false, JuggKills >= 4);

            if (interact[3])
                JuggKills++;

            if (JuggKills == 4 && Local)
                Flash(Color);

            if (interact[0])
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
            base.UpdateHud(__instance);
            AssaultButton.Update("ASSAULT", AssaultTimer(), CustomGameOptions.JuggKillCooldown, -(CustomGameOptions.JuggKillBonus * JuggKills));
        }
    }
}