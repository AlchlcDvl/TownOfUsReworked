using Il2CppSystem.Collections.Generic;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Executioner : Role
    {
        public PlayerControl TargetPlayer = null;
        public bool TargetVotedOut;

        public Executioner(PlayerControl player) : base(player)
        {
            Name = "Executioner";
            StartText = "Eject Your Target";
            Objectives = "- Eject your target.";
            Color = IsRecruit ? Colors.Cabal : (CustomGameOptions.CustomNeutColors ? Colors.Executioner : Colors.Neutral);
            RoleType = RoleEnum.Executioner;
            Faction = Faction.Neutral;
            FactionName = "Neutral";
            FactionColor = Colors.Neutral;
            RoleAlignment = RoleAlignment.NeutralEvil;
            AlignmentName = "Neutral (Evil)";
            Results = InspResults.GAExeMedicPup;
            FactionDescription = NeutralFactionDescription;
            AlignmentDescription = NEDescription;
            RoleDescription = "You are an Executioner! You are a crazed stalker who only wants to see your target get ejected. Eject them at all costs!";
            AddToRoleHistory(RoleType);
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var team = new List<PlayerControl>();

            team.Add(PlayerControl.LocalPlayer);

            if (IsRecruit)
            {
                var jackal = Player.GetJackal();

                team.Add(jackal.Player);
                team.Add(jackal.EvilRecruit);
            }
            
            team.Add(TargetPlayer);

            __instance.teamToShow = team;
        }

        public override void Wins()
        {
            if ((Player.Data.IsDead && !CustomGameOptions.ExeCanWinBeyondDeath) || Player.Data.Disconnected || TargetPlayer == null)
                return;
                
            TargetVotedOut = true;
        }

        public override void Loses()
        {
            LostByRPC = true;
        }
    }
}