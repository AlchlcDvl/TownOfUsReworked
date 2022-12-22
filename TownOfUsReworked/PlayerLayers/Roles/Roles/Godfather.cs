using System.Linq;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Patches;
using Il2CppSystem.Collections.Generic;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Godfather : Role
    {
        public PlayerControl ClosestIntruder;
        public bool HasDeclared = false;
        public bool WasMafioso = false;
        public Role FormerRole = null;
        public KillButton _declareButton;

        public Godfather(PlayerControl player) : base(player)
        {
            Name = "Godfather";
            Faction = Faction.Intruders;
            RoleType = RoleEnum.Godfather;
            StartText = "Promote Your Fellow <color=#FF0000FF>Intruders</color> To Do Better";
            AbilitiesText = "- You can promote a fellow <color=#FF0000FF>Intruder</color> into becoming your successor.";
            AttributesText = "- Promoting an <color=#FF0000FF>Intruder</color> turns them into a <color=#6400FFFF>Mafioso</color>.\n- If you die, " +
                "the <color=#6400FFFF>Mafioso</color> become the new <color=#404C08FF>Godfather</color>\nand inherits better abilities of their former" +
                " role.";
            Color = CustomGameOptions.CustomImpColors ? Colors.Godfather : Colors.Intruder;
            FactionName = "Intruder";
            FactionColor = Colors.Intruder;
            RoleAlignment = RoleAlignment.IntruderSupport;
            AlignmentName = "Intruder (Support)";
            IntroText = "Kill those who oppose you";
            Results = InspResults.GFMayorRebelPest;
            IntroSound = null;
            Attack = AttackEnum.Basic;
            Defense = DefenseEnum.None;
            AttackString = "Basic";
            DefenseString = "None";
            FactionDescription = "You are an Intruder! Your main task is to kill anyone who dares to oppose you. Sabotage the systems, murder the crew, do anything" +
                " to ensure your victory over others.";
            Objectives = "- Kill: <color=#008000FF>Syndicate</color>, <color=#8BFDFD>Crew</color> and <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killers</color>," +
                " <color=#1D7CF2FF>Proselytes</color> and <color=#1D7CF2FF>Neophytes</color>.\n   or\n- Have a critical sabotage reach 0 seconds.";
            AlignmentDescription = "You are a Intruder (Support) role! It is your job to ensure no one bats an eye to the things you or your mates do. Support them in " +
                "everyway possible.";
            RoleDescription = "You are a Godfather! You are the leader of the Intruders. You can promote a fellow Intruder into becoming your Mafioso." +
                " When you die, the Mafioso will become the new Godfather and will inherit stronger variations of their former role.";
            AddToRoleHistory(RoleType);
        }

        public override void Wins()
        {
            IntruderWin = true;
        }

        public override void Loses()
        {
            LostByRPC = true;
        }

        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (Player.Data.IsDead | Player.Data.Disconnected)
                return true;

            if ((PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(Faction.Crew) |
                x.Is(RoleAlignment.NeutralKill) | x.Is(Faction.Syndicate) | x.Is(RoleAlignment.NeutralNeo) | x.Is(RoleAlignment.NeutralPros))) == 0) |
                Utils.Sabotaged())
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.IntruderWin,
                    SendOption.Reliable, -1);
                writer.Write(Player.PlayerId);
                Wins();
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            
            return false;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var intTeam = new List<PlayerControl>();

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.Is(Faction.Intruders))
                    intTeam.Add(player);
            }
            __instance.teamToShow = intTeam;
        }

        public KillButton DeclareButton
        {
            get => _declareButton;
            set
            {
                _declareButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
    }
}