namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Pestilence : Neutral
    {
        public DateTime LastKilled;
        public CustomButton ObliterateButton;

        public override Color32 Color => ClientGameOptions.CustomNeutColors ? Colors.Pestilence : Colors.Neutral;
        public override string Name => "Pestilence";
        public override LayerEnum Type => LayerEnum.Pestilence;
        public override RoleEnum RoleType => RoleEnum.Pestilence;
        public override Func<string> StartText => () => "THE APOCALYPSE IS NIGH";
        public override Func<string> AbilitiesText => () => "- You are on forever alert, anyone who interacts with you will be killed";
        public override InspectorResults InspectorResults => InspectorResults.LeadsTheGroup;

        public Pestilence(PlayerControl owner) : base(owner)
        {
            Objectives = () => "- Obliterate anyone who can oppose you";
            RoleAlignment = RoleAlignment.NeutralApoc;
            ObliterateButton = new(this, "Obliterate", AbilityTypes.Direct, "ActionSecondary", Obliterate, Exception);
        }

        public float ObliterateTimer()
        {
            var timespan = DateTime.UtcNow - LastKilled;
            var num = Player.GetModifiedCooldown(CustomGameOptions.PestKillCd) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public void Obliterate()
        {
            if (IsTooFar(Player, ObliterateButton.TargetPlayer) || ObliterateTimer() != 0f)
                return;

            var interact = Interact(Player, ObliterateButton.TargetPlayer, true);

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
            base.UpdateHud(__instance);
            ObliterateButton.Update("OBLITERATE", ObliterateTimer(), CustomGameOptions.PestKillCd);
        }
    }
}