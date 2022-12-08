using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using Il2CppSystem.Collections.Generic;
using TownOfUsReworked.Extensions;
using Hazel;
using System.Linq;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Crewmate : Role
    {
        public Crewmate(PlayerControl player) : base(player)
        {
            Name = "Crewmate";
            Faction = Faction.Crew;
            RoleType = RoleEnum.Crewmate;
            StartText = "Do Your Tasks";
            AbilitiesText = "- None.";
            AttributesText = "- None.";
            Color = Colors.Crew;
            FactionName = "Crew";
            FactionColor = Colors.Crew;
            RoleAlignment = RoleAlignment.CrewUtil;
            AlignmentName = "Crew (Utility)";
            IntroText = "Eject all <color=#FF0000FF>evildoers</color>";
            Results = InspResults.CrewImpAnMurd;
            SubFaction = SubFaction.None;
            IntroSound = TownOfUsReworked.CrewmateIntro;
            Base = true;
            Attack = AttackEnum.None;
            Defense = DefenseEnum.None;
            AttackString = "None";
            DefenseString = "None";
            AlignmentDescription = "You are a Crew (Utility) role! You usually have no special ability and cannot even appear under natural conditions.";
            RoleDescription = "You are a Crewmate! Your role is the base role for the Crew faction. You have no special abilities and should probably do your tasks.";
            FactionDescription = "Your faction is the Crew! You do not know who the other members of your faction are. It is your job to deduce" + 
                " who is evil and who is not. Eject or kill all evils or finish all of your tasks to win!";
            Objectives = "- Finish your tasks along with other Crew.\n   or\n- Kill: <color=#FF0000FF>Intruders</color>, <color=#008000FF>Syndicate</color>" + 
                " and <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killers</color>, <color=#1D7CF2FF>Proselytes</color> and " +
                "<color=#1D7CF2FF>Neophytes</color>.";
            AddToRoleHistory(RoleType);
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var team = new List<PlayerControl>();
            team.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = team;
        }

        public override void Wins()
        {
            CrewWin = true;
        }

        public override void Loses()
        {
            LostByRPC = true;
        }

        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (Player.Data.IsDead | Player.Data.Disconnected)
                return true;

            if ((PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Data.IsImpostor() |
                x.Is(RoleAlignment.NeutralKill) | x.Is(RoleAlignment.NeutralNeo) | x.Is(Faction.Syndicate) | x.Is(RoleAlignment.NeutralPros))) ==
                0) | Utils.TasksDone())
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CrewWin,
                    SendOption.Reliable, -1);
                writer.Write(Player.PlayerId);
                Wins();
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }

            return false;
        }
    }
}