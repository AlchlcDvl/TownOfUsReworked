namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class SerialKiller : NeutralRole
    {
        public CustomButton BloodlustButton;
        public CustomButton StabButton;
        public bool Enabled;
        public DateTime LastLusted;
        public DateTime LastKilled;
        public float TimeRemaining;
        public bool Lusted => TimeRemaining > 0f;

        public SerialKiller(PlayerControl player) : base(player)
        {
            Name = "Serial Killer";
            StartText = () => "You Like To Play With Knives";
            AbilitiesText = () => "- You can go into bloodlust\n- When in bloodlust, your kill cooldown is very short\n- If and when an <color=#803333FF>Escort</color>, " +
                "<color=#801780FF>Consort</color> or <color=#00FF00FF>Glitch</color> tries to block you, you will immediately kill them, regardless of your cooldown\n- You are " +
                "immune to blocks";
            Objectives = () => "- Stab anyone who can oppose you";
            Color = CustomGameOptions.CustomNeutColors ? Colors.SerialKiller : Colors.Neutral;
            RoleType = RoleEnum.SerialKiller;
            RoleAlignment = RoleAlignment.NeutralKill;
            RoleBlockImmune = true;
            Type = LayerEnum.SerialKiller;
            StabButton = new(this, "Stab", AbilityTypes.Direct, "ActionSecondary", Stab, Exception);
            BloodlustButton = new(this, "Bloodlust", AbilityTypes.Effect, "Secondary", Lust);
            InspectorResults = InspectorResults.IsAggressive;

            if (TownOfUsReworked.IsTest)
                Utils.LogSomething($"{Player.name} is {Name}");
        }

        public float LustTimer()
        {
            var timespan = DateTime.UtcNow - LastLusted;
            var num = Player.GetModifiedCooldown(CustomGameOptions.BloodlustCd) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Bloodlust()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;

            if (IsDead || MeetingHud.Instance)
                TimeRemaining = 0f;
        }

        public void Unbloodlust()
        {
            Enabled = false;
            LastLusted = DateTime.UtcNow;
        }

        public float StabTimer()
        {
            var timespan = DateTime.UtcNow - LastKilled;
            var num = Player.GetModifiedCooldown(CustomGameOptions.LustKillCd) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Lust()
        {
            if (LustTimer() != 0f || Lusted)
                return;

            TimeRemaining = CustomGameOptions.BloodlustDuration;
            Bloodlust();
        }

        public void Stab()
        {
            if (!Lusted || StabTimer() != 0f || Utils.IsTooFar(Player, StabButton.TargetPlayer))
                return;

            var interact = Utils.Interact(Player, StabButton.TargetPlayer, true);

            if (interact[3] || interact[0])
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
            StabButton.Update("STAB", StabTimer(), CustomGameOptions.LustKillCd, Lusted, Lusted);
            BloodlustButton.Update("BLOODLUST", LustTimer(), CustomGameOptions.BloodlustCd, Lusted, TimeRemaining, CustomGameOptions.BloodlustDuration);
        }
    }
}
