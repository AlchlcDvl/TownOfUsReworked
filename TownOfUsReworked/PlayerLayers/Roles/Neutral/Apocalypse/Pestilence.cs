namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Pestilence : NeutralRole
    {
        public DateTime LastKilled;
        public CustomButton ObliterateButton;

        public Pestilence(PlayerControl owner) : base(owner)
        {
            Name = "Pestilence";
            StartText = "The Horseman Of The Apocalypse Has Arrived!";
            AbilitiesText = "- You are on forever alert, anyone who interacts with you will be killed";
            Objectives = "- Obliterate anyone who can oppose you";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Pestilence : Colors.Neutral;
            RoleType = RoleEnum.Pestilence;
            RoleAlignment = RoleAlignment.NeutralApoc;
            Type = LayerEnum.Pestilence;
            ObliterateButton = new(this, "Obliterate", AbilityTypes.Direct, "ActionSecondary", Obliterate, Exception);
            InspectorResults = InspectorResults.LeadsTheGroup;
        }

        public float ObliterateTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastKilled;
            var num = Player.GetModifiedCooldown(CustomGameOptions.PestKillCd) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Obliterate()
        {
            if (Utils.IsTooFar(Player, ObliterateButton.TargetPlayer) || ObliterateTimer() != 0f)
                return;

            var interact = Utils.Interact(Player, ObliterateButton.TargetPlayer, true);

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
            ObliterateButton.Update("OBLITERATE", ObliterateTimer(), CustomGameOptions.PestKillCd);
        }
    }
}