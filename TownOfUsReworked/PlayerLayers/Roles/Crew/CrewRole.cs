namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class CrewRole : Role
    {
        protected CrewRole(PlayerControl player) : base(player)
        {
            Faction = Faction.Crew;
            FactionColor = Colors.Crew;
            Color = Colors.Crew;
            Objectives = () => CrewWinCon;
            BaseFaction = Faction.Crew;
            Player.Data.SetImpostor(false);
        }

        public override void IntroPrefix(IntroCutscene._ShowTeam_d__36 __instance)
        {
            if (!Local)
                return;

            var team = new List<PlayerControl> { PlayerControl.LocalPlayer };

            if (IsRecruit)
            {
                var jackal = Player.GetJackal();

                team.Add(jackal.Player);
                team.Add(jackal.EvilRecruit);
            }

            if (Player.Is(ObjectifierEnum.Lovers))
                team.Add(Player.GetOtherLover());
            else if (Player.Is(ObjectifierEnum.Rivals))
                team.Add(Player.GetOtherRival());
            else if (Player.Is(ObjectifierEnum.Mafia))
            {
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player != Player && player.Is(ObjectifierEnum.Mafia))
                        team.Add(player);
                }
            }

            __instance.teamToShow = team.SystemToIl2Cpp();
        }
    }
}