namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Inspector : Crew
    {
        public DateTime LastInspected;
        public List<byte> Inspected = new();
        public CustomButton InspectButton;

        public override Color32 Color => ClientGameOptions.CustomCrewColors ? Colors.Inspector : Colors.Crew;
        public override string Name => "Inspector";
        public override LayerEnum Type => LayerEnum.Inspector;
        public override RoleEnum RoleType => RoleEnum.Inspector;
        public override Func<string> StartText => () => "Inspect Players For Their Roles";
        public override Func<string> AbilitiesText => () => "- You can check a player to get a role list of what they could be";
        public override InspectorResults InspectorResults => InspectorResults.GainsInfo;

        public Inspector(PlayerControl player) : base(player)
        {
            RoleAlignment = RoleAlignment.CrewInvest;
            Inspected = new();
            InspectButton = new(this, "Inspect", AbilityTypes.Direct, "ActionSecondary", Inspect, Exception);
        }

        public float InspectTimer()
        {
            var timespan = DateTime.UtcNow - LastInspected;
            var num = Player.GetModifiedCooldown(CustomGameOptions.InspectCooldown) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public void Inspect()
        {
            if (InspectTimer() != 0f || IsTooFar(Player, InspectButton.TargetPlayer) || Inspected.Contains(InspectButton.TargetPlayer.PlayerId))
                return;

            var interact = Interact(Player, InspectButton.TargetPlayer);

            if (interact[3])
                Inspected.Add(InspectButton.TargetPlayer.PlayerId);

            if (interact[0])
                LastInspected = DateTime.UtcNow;
            else if (interact[1])
                LastInspected.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public bool Exception(PlayerControl player) => Inspected.Contains(player.PlayerId) || (((Faction is Faction.Intruder or Faction.Syndicate && player.Is(Faction)) ||
            (player.Is(SubFaction) && SubFaction != SubFaction.None)) && CustomGameOptions.FactionSeeRoles) || (Player.IsOtherLover(player) && CustomGameOptions.LoversRoles) ||
            (Player.IsOtherRival(player) && CustomGameOptions.RivalsRoles) || (player.Is(ObjectifierEnum.Mafia) && Player.Is(ObjectifierEnum.Mafia) && CustomGameOptions.MafiaRoles) ||
            (Player.IsOtherLink(player) && CustomGameOptions.LinkedRoles);

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            InspectButton.Update("INSPECT", InspectTimer(), CustomGameOptions.InspectCooldown);
        }
    }
}