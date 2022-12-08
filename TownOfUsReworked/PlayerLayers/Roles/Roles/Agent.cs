using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Lobby.CustomOption;
using Il2CppSystem.Collections.Generic;
using TownOfUsReworked.Extensions;
using Hazel;
using System.Linq;
using TMPro;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Agent : Role
    {
        public System.Collections.Generic.Dictionary<byte, TMP_Text> PlayerNumbers = new System.Collections.Generic.Dictionary<byte, TMP_Text>();

        public Agent(PlayerControl player) : base(player)
        {
            Name = "Agent";
            StartText = "Snoop Around And Find Stuff Out";
            AbilitiesText = "- You can see which colors are where on the admin table.";
            AttributesText = "- None.";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Agent : Colors.Crew;
            RoleType = RoleEnum.Agent;
            Faction = Faction.Crew;
            FactionName = "Crew";
            FactionColor = Colors.Crew;
            RoleAlignment = RoleAlignment.CrewInvest;
            AlignmentName = "Crew (Investigative)";
            IntroText = "Eject all <color=#FF0000FF>evildoers</color>";
            CoronerDeadReport = "The technology this one is carrying indicates that this body is an Agent!";
            CoronerKillerReport = "";
            Results = InspResults.DisgMorphCamoAgent;
            SubFaction = SubFaction.None;
            IntroSound = TownOfUsReworked.AgentIntro;
            Attack = AttackEnum.None;
            AttackString = "None";
            Defense = DefenseEnum.None;
            DefenseString = "None";
            FactionDescription = "Your faction is the Crew! You do not know who the other members of your faction are. It is your job to deduce" + 
                " who is evil and who is not. Eject or kill all evils or finish all of your tasks to win!";
            AlignmentDescription = "You are a Crew (Investigative) role! You can gain information via special methods and using that acquired info, you" +
                " can deduce who is good and who is not.";
            RoleDescription = "Your are an Agent! You can see extra information from the admin table. When active, all players in detectable rooms" +
                " will have their color revealed to you.";
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

            if ((PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(Faction.Intruders) |
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