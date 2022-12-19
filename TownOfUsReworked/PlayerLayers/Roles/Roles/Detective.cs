using System;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using Hazel;
using System.Linq;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Detective : Role
    {
        public PlayerControl ClosestPlayer;
        public DateTime LastExamined { get; set; }

        public Detective(PlayerControl player) : base(player)
        {
            Name = "Detective";
            StartText = "Examine Players To Find Bloody Hands";
            AbilitiesText = "- You can examine players to see if they have killed recently.";
            AttributesText = $"- Your screen will flash red if your target has killed in the last {CustomGameOptions.RecentKill}s.";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Detective : Colors.Crew;
            LastExamined = DateTime.UtcNow;
            RoleType = RoleEnum.Detective;
            Faction = Faction.Crew;
            FactionName = "Crew";
            FactionColor = Colors.Crew;
            RoleAlignment = RoleAlignment.CrewInvest;
            AlignmentName = "Crew (Investigative)";
            IntroText = "Eject all <color=#FF0000FF>evildoers</color>";
            CoronerDeadReport = "There are documents pertaining to everyone's activity on the body. They must be a Detective!";
            CoronerKillerReport = "";
            Results = InspResults.WraithDetGrenVet;
            SubFaction = SubFaction.None;
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
            RoleDescription = "You are a Detective! You have a special skill in identifying blood on others. Use this to your advantage to catch killers" +
                " in the act!";
            Base = false;
            IsRecruit = false;
            AddToRoleHistory(RoleType);
        }

        public float ExamineTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastExamined;
            var num = CustomGameOptions.ExamineCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;
                
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__32 __instance)
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