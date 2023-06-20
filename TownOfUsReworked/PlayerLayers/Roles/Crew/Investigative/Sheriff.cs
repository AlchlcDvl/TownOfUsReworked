namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Sheriff : Crew
    {
        public CustomButton InterrogateButton;
        public DateTime LastInterrogated;

        public Sheriff(PlayerControl player) : base(player)
        {
            Name = "Sheriff";
            StartText = () => "Reveal The Alignment Of Other Players";
            AbilitiesText = () => "- You can reveal alignments of other players relative to the <color=#8CFFFFFF>Crew</color>";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Sheriff : Colors.Crew;
            RoleType = RoleEnum.Sheriff;
            RoleAlignment = RoleAlignment.CrewKill;
            InspectorResults = InspectorResults.GainsInfo;
            Type = LayerEnum.Sheriff;
            InterrogateButton = new(this, "Interrogate", AbilityTypes.Direct, "ActionSecondary", Interrogate, Exception);

            if (TownOfUsReworked.IsTest)
                Utils.LogSomething($"{Player.name} is {Name}");
        }

        public float InterrogateTimer()
        {
            var timespan = DateTime.UtcNow - LastInterrogated;
            var num = Player.GetModifiedCooldown(CustomGameOptions.InterrogateCd) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Interrogate()
        {
            if (InterrogateTimer() != 0f || Utils.IsTooFar(Player, InterrogateButton.TargetPlayer))
                return;

            var interact = Utils.Interact(Player, InterrogateButton.TargetPlayer);

            if (interact[3])
            {
                if (InterrogateButton.TargetPlayer.SeemsEvil())
                    Utils.Flash(new(255, 0, 0, 255));
                else
                    Utils.Flash(new(0, 255, 0, 255));
            }

            if (interact[0])
                LastInterrogated = DateTime.UtcNow;
            else if (interact[1])
                LastInterrogated.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public bool Exception(PlayerControl player) => (((Faction is Faction.Intruder or Faction.Syndicate && player.Is(Faction)) || (player.Is(SubFaction) && SubFaction !=
            SubFaction.None)) && CustomGameOptions.FactionSeeRoles) || (player == Player.GetOtherLover() && CustomGameOptions.LoversRoles) || (player == Player.GetOtherRival() &&
            CustomGameOptions.RivalsRoles) || (player.Is(ObjectifierEnum.Mafia) && Player.Is(ObjectifierEnum.Mafia) && CustomGameOptions.MafiaRoles);

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            InterrogateButton.Update("INTERROGATE", InterrogateTimer(), CustomGameOptions.InterrogateCd);
        }
    }
}