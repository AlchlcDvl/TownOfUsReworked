namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Sheriff : Crew
    {
        public CustomButton InterrogateButton { get; set; }
        public DateTime LastInterrogated { get; set; }
        public float Timer => ButtonUtils.Timer(Player, LastInterrogated, CustomGameOptions.InterrogateCd);

        public override Color32 Color => ClientGameOptions.CustomCrewColors ? Colors.Sheriff : Colors.Crew;
        public override string Name => "Sheriff";
        public override LayerEnum Type => LayerEnum.Sheriff;
        public override RoleEnum RoleType => RoleEnum.Sheriff;
        public override Func<string> StartText => () => "Reveal The Alignment Of Other Players";
        public override Func<string> Description => () => "- You can reveal alignments of other players relative to the <color=#8CFFFFFF>Crew</color>";
        public override InspectorResults InspectorResults => InspectorResults.GainsInfo;

        public Sheriff(PlayerControl player) : base(player)
        {
            RoleAlignment = RoleAlignment.CrewKill;
            InterrogateButton = new(this, "Interrogate", AbilityTypes.Direct, "ActionSecondary", Interrogate, Exception);
        }

        public void Interrogate()
        {
            if (Timer != 0f || IsTooFar(Player, InterrogateButton.TargetPlayer))
                return;

            var interact = Interact(Player, InterrogateButton.TargetPlayer);

            if (interact[3])
                Flash(InterrogateButton.TargetPlayer.SeemsEvil() ? UColor.red : UColor.green);

            if (interact[0])
                LastInterrogated = DateTime.UtcNow;
            else if (interact[1])
                LastInterrogated.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public bool Exception(PlayerControl player) => (((Faction is Faction.Intruder or Faction.Syndicate && player.Is(Faction)) || (player.Is(SubFaction) && SubFaction !=
            SubFaction.None)) && CustomGameOptions.FactionSeeRoles) || (Player.IsOtherLover(player) && CustomGameOptions.LoversRoles) || (Player.IsOtherRival(player) &&
            CustomGameOptions.RivalsRoles) || (player.Is(ObjectifierEnum.Mafia) && Player.Is(ObjectifierEnum.Mafia) && CustomGameOptions.MafiaRoles) || (Player.IsOtherLink(player) &&
            CustomGameOptions.LinkedRoles);

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            InterrogateButton.Update("INTERROGATE", Timer, CustomGameOptions.InterrogateCd);
        }
    }
}