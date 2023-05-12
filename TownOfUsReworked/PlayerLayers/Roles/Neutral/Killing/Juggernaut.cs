namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Juggernaut : NeutralRole
    {
        public DateTime LastKilled;
        public int JuggKills;
        public CustomButton AssaultButton;

        public Juggernaut(PlayerControl player) : base(player)
        {
            Name = "Juggernaut";
            StartText = "Your Power Grows With Every Kill";
            AbilitiesText = "- With each kill, your kill cooldown decreases\n- At 4 kills, you bypass all forms of protection";
            Objectives = "- Assault anyone who can oppose you";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Juggernaut : Colors.Neutral;
            RoleType = RoleEnum.Juggernaut;
            RoleAlignment = RoleAlignment.NeutralKill;
            AlignmentName = NK;
            JuggKills = 0;
            Type = LayerEnum.Juggernaut;
            AssaultButton = new(this, "Assault", AbilityTypes.Direct, "ActionSecondary", Assault, Exception);
            InspectorResults = InspectorResults.IsAggressive;
        }

        public float AssaultTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastKilled;
            var num = Mathf.Clamp(Player.GetModifiedCooldown(CustomGameOptions.JuggKillCooldown, -(CustomGameOptions.JuggKillBonus * JuggKills)), 5,
                Player.GetModifiedCooldown(CustomGameOptions.JuggKillCooldown)) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Assault()
        {
            if (Utils.IsTooFar(Player, AssaultButton.TargetPlayer) || AssaultTimer() != 0f)
                return;

            var interact = Utils.Interact(Player, AssaultButton.TargetPlayer, true, false, JuggKills >= 4);

            if (interact[3])
                JuggKills++;

            if (JuggKills == 4 && PlayerControl.LocalPlayer == Player)
                Utils.Flash(Color);

            if (interact[0])
                LastKilled = DateTime.UtcNow;
            else if (interact[1])
                LastKilled.AddSeconds(CustomGameOptions.ProtectKCReset);
            else if (interact[2])
                LastKilled.AddSeconds(CustomGameOptions.VestKCReset);
        }

        public bool Exception(PlayerControl player) => (player.Is(SubFaction) && SubFaction != SubFaction.None) || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) ||
            player == Player.GetOtherLover() || player == Player.GetOtherRival() || (player.Is(ObjectifierEnum.Mafia) && Player.Is(ObjectifierEnum.Mafia));

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            AssaultButton.Update("ASSAULT", AssaultTimer(), Mathf.Clamp(Player.GetModifiedCooldown(CustomGameOptions.JuggKillCooldown, -(CustomGameOptions.JuggKillBonus * JuggKills)), 5,
                Player.GetModifiedCooldown(CustomGameOptions.JuggKillCooldown)));
        }
    }
}