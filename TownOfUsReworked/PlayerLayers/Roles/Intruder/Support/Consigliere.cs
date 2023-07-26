namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Consigliere : Intruder
    {
        public List<byte> Investigated = new();
        public CustomButton InvestigateButton;
        public DateTime LastInvestigated;
        private static string Option => CustomGameOptions.ConsigInfo == ConsigInfo.Role ? "role" : "faction";
        private string CanAssassinate => Player.Is(LayerEnum.Assassin, PlayerLayerEnum.Ability) && CustomGameOptions.ConsigInfo == ConsigInfo.Role ? ("\n- You cannot assassinate players " +
            "you have revealed") : "";

        public override Color32 Color => ClientGameOptions.CustomIntColors ? Colors.Consigliere : Colors.Intruder;
        public override string Name => "Consigliere";
        public override LayerEnum Type => LayerEnum.Consigliere;
        public override RoleEnum RoleType => RoleEnum.Consigliere;
        public override Func<string> StartText => () => "See The <color=#8CFFFFFF>Crew</color> For Who They Really Are";
        public override Func<string> AbilitiesText => () => $"- You can reveal a player's {Option}{CanAssassinate}\n{CommonAbilities}";
        public override InspectorResults InspectorResults => InspectorResults.GainsInfo;

        public Consigliere(PlayerControl player) : base(player)
        {
            RoleAlignment = RoleAlignment.IntruderSupport;
            Investigated = new();
            InvestigateButton = new(this, "Investigate", AbilityTypes.Direct, "Secondary", Investigate, Exception1);
        }

        public float ConsigliereTimer()
        {
            var timespan = DateTime.UtcNow - LastInvestigated;
            var num = Player.GetModifiedCooldown(CustomGameOptions.ConsigCd) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public void Investigate()
        {
            if (ConsigliereTimer() != 0f || IsTooFar(Player, InvestigateButton.TargetPlayer) || Investigated.Contains(InvestigateButton.TargetPlayer.PlayerId))
                return;

            var interact = Interact(Player, InvestigateButton.TargetPlayer);

            if (interact[3])
                Investigated.Add(InvestigateButton.TargetPlayer.PlayerId);

            if (interact[0])
                LastInvestigated = DateTime.UtcNow;
            else if (interact[1])
                LastInvestigated.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public bool Exception1(PlayerControl player) => Investigated.Contains(player.PlayerId) || (((Faction is Faction.Intruder or Faction.Syndicate && player.Is(Faction)) ||
            (player.Is(SubFaction) && SubFaction != SubFaction.None)) && CustomGameOptions.FactionSeeRoles) || (Player.IsOtherLover(player) && CustomGameOptions.LoversRoles) ||
            (Player.IsOtherRival(player) && CustomGameOptions.RivalsRoles) || (player.Is(ObjectifierEnum.Mafia) && Player.Is(ObjectifierEnum.Mafia) && CustomGameOptions.MafiaRoles) ||
            (Player.IsOtherLink(player) && CustomGameOptions.LinkedRoles);

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            InvestigateButton.Update("INVESTIGATE", ConsigliereTimer(), CustomGameOptions.ConsigCd);
        }
    }
}