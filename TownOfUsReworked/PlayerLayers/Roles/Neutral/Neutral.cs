namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Neutral : Role
    {
        public override Color32 Color => Colors.Neutral;
        public override Faction BaseFaction => Faction.Neutral;

        protected Neutral(PlayerControl player) : base(player)
        {
            Faction = Faction.Neutral;
            FactionColor = Colors.Neutral;
            Player.Data.SetImpostor(false);
        }

        public override void IntroPrefix(IntroCutscene._ShowTeam_d__36 __instance)
        {
            if (!Local)
                return;

            var team = new List<PlayerControl> { CustomPlayer.Local };

            if (IsRecruit)
            {
                var jackal = Player.GetJackal();
                team.Add(jackal.Player);
                team.Add(Player == jackal.EvilRecruit ? jackal.GoodRecruit : jackal.EvilRecruit);
            }
            else if (RoleType == RoleEnum.Jackal)
            {
                var jackal = (Jackal)this;
                team.Add(jackal.GoodRecruit);
                team.Add(jackal.EvilRecruit);
            }

            if (HasTarget && RoleType != RoleEnum.BountyHunter)
                team.Add(Player.GetTarget());

            if (Player.Is(ObjectifierEnum.Lovers))
                team.Add(Player.GetOtherLover());
            else if (Player.Is(ObjectifierEnum.Rivals))
                team.Add(Player.GetOtherRival());
            else if (Player.Is(ObjectifierEnum.Linked))
                team.Add(Player.GetOtherLink());
            else if (Player.Is(ObjectifierEnum.Mafia))
            {
                foreach (var player in CustomPlayer.AllPlayers)
                {
                    if (player != Player && player.Is(ObjectifierEnum.Mafia))
                        team.Add(player);
                }
            }
            else if (Player.Is(ObjectifierEnum.Allied) && !Player.Is(Faction.Crew))
            {
                foreach (var player in CustomPlayer.AllPlayers)
                {
                    if (player.Is(Faction) && player != Player)
                        team.Add(player);
                }
            }

            __instance.teamToShow = team.SystemToIl2Cpp();
        }
    }
}