using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Lobby.CustomOption;
using Il2CppSystem.Collections.Generic;
using System;
using TownOfUsReworked.Extensions;
using Hazel;
using System.Linq;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Inspector : Role
    {
        public PlayerControl ClosestPlayer;
        public DateTime LastExamined { get; set; }
        public List<PlayerControl> Examined = new List<PlayerControl>();

        public Inspector(PlayerControl player) : base(player)
        {
            Name = "Inspector";
            Faction = Faction.Crew;
            RoleType = RoleEnum.Inspector;
            StartText = "Inspect Player For Their Roles";
            AbilitiesText = "See if people really are what they claim to be";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Inspector : Colors.Crew;
            FactionName = "Crew";
            FactionColor = Colors.Crew;
            RoleAlignment = RoleAlignment.CrewInvest;
            AlignmentName = "Crew (Investigative)";
            IntroText = "Eject all <color=#FF0000FF>evildoers</color>";
            CoronerDeadReport = "There are documents pertaining to everyone's identities on the body. They must be an Inspector!";
            CoronerKillerReport = "";
            Results = InspResults.SherConsigInspBm;
            Attack = AttackEnum.None;
            Defense = DefenseEnum.None;
            AttackString = "None";
            DefenseString = "None";
            FactionDescription = "Your faction is the Crew! You do not know who the other members of your faction are. It is your job to deduce" + 
                " who is evil and who is not. Eject or kill all evils or finish all of your tasks to win!";
            AlignmentDescription = "You are a Crew (Investigative) role! You can gain information via special methods and using that acquired info, you" +
                " can deduce who is good and who is not.";
            RoleDescription = "You are an Inspector! You can inspect players to see a role list of what they could be. If someone's claim is not in that " +
                "list, they are not Crew.";
            AddToRoleHistory(RoleType);
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var team = new List<PlayerControl>();
            team.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = team;
        }

        public float ExamineTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastExamined;
            var num = CustomGameOptions.InspectCooldown * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;
                
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
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