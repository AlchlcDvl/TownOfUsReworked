using System.Collections.Generic;
using System.Linq;
using GameObject = UnityEngine.Object;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Lobby.CustomOption;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Coroner : Role
    {
        public Dictionary<byte, ArrowBehaviour> BodyArrows = new Dictionary<byte, ArrowBehaviour>();
        public readonly List<GameObject> Buttons = new List<GameObject>();
        public List<byte> Reported = new List<byte>();

        public Coroner(PlayerControl player) : base(player)
        {
            Name = "Coroner";
            StartText = "Examine The Dead For Info";
            AbilitiesText = "- You know when players die and for a brief period know where their body is.";
            AttributesText = "- When reporting a body, you get all of therequired info from it.";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Coroner : Colors.Crew;
            RoleType = RoleEnum.Coroner;
            Faction = Faction.Crew;
            FactionName = "Crew";
            FactionColor = Colors.Crew;
            RoleAlignment = RoleAlignment.CrewInvest;
            AlignmentName = "Crew (Investigative)";
            IntroText = "Eject all <color=#FF0000FF>evildoers</color>";
            CoronerDeadReport = "The scalpels and human matter on the body indicate that this body is another Coroner!";
            CoronerKillerReport = "";
            Results = InspResults.CoroJaniUTMed;
            IntroSound = null;
            Attack = AttackEnum.None;
            Defense = DefenseEnum.None;
            AttackString = "None";
            DefenseString = "None";
            FactionDescription = "Your faction is the Crew! You do not know who the other members of your faction are. It is your job to deduce" + 
                " who is evil and who is not. Eject or kill all evils or finish all of your tasks to win!";
            Objectives = "- Finish your tasks along with other Crew.\n   or\n- Kill: <color=#FF0000FF>Intruders</color>, <color=#008000FF>Syndicate</color>" + 
                " and <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killers</color>, <color=#1D7CF2FF>Proselytes</color> and " +
                "<color=#1D7CF2FF>Neophytes</color>.";
            AlignmentDescription = "You are a Crew (Investigative) role! You can gain information via special methods and using that acquired info, you" +
                " can deduce who is good and who is not.";
            RoleDescription = "You are a Coroner! You are an expert in revealing information from dead bodies to the point you even know when someone dies!" +
                " Your strong ability makes you a very tempting target for evils so be careful when revealing information.";
            AddToRoleHistory(RoleType);
        }

        public void DestroyArrow(byte targetPlayerId)
        {
            var arrow = BodyArrows.FirstOrDefault(x => x.Key == targetPlayerId);

            if (arrow.Value != null)
                GameObject.Destroy(arrow.Value);
            if (arrow.Value.gameObject != null)
                GameObject.Destroy(arrow.Value.gameObject);
                
            BodyArrows.Remove(arrow.Key);
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var team = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
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

            if (Utils.CrewWins())
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