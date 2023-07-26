namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Murderer : Neutral
    {
        public DateTime LastKilled;
        public CustomButton MurderButton;

        public override Color32 Color => ClientGameOptions.CustomNeutColors ? Colors.Murderer : Colors.Neutral;
        public override string Name => "Murderer";
        public override LayerEnum Type => LayerEnum.Murderer;
        public override RoleEnum RoleType => RoleEnum.Murderer;
        public override Func<string> StartText => () => "I Got Murder On My Mind";
        public override Func<string> AbilitiesText => () => "- You can kill";
        public override InspectorResults InspectorResults => InspectorResults.IsBasic;

        public Murderer(PlayerControl player) : base(player)
        {
            Objectives = () => "- Murder anyone who can oppose you";
            RoleAlignment = RoleAlignment.NeutralKill;
            MurderButton = new(this, "Murder", AbilityTypes.Direct, "ActionSecondary", Murder, Exception);
        }

        public float MurderTimer()
        {
            var timespan = DateTime.UtcNow - LastKilled;
            var num = Player.GetModifiedCooldown(CustomGameOptions.MurdKCD) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public void Murder()
        {
            if (IsTooFar(Player, MurderButton.TargetPlayer) || MurderTimer() != 0f)
                return;

            var interact = Interact(Player, MurderButton.TargetPlayer, true);

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
            MurderButton.Update("MURDER", MurderTimer(), CustomGameOptions.MurdKCD);
        }
    }
}