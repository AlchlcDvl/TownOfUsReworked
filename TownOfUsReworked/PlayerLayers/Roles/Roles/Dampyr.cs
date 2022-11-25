using System;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Patches;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Dampyr : Role
    {
        public PlayerControl ClosestPlayer;
        public DateTime LastKill { get; set; }
        public bool DampWins { get; set; }

        public Dampyr(PlayerControl player) : base(player)
        {
            Name = "Dampyr";
            ImpostorText = () => "Kill Off The Crew To Gain A Majority";
            TaskText = () => "Kill everyone and help the Dracula reach majority!\nFake Tasks:";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Dampyr : Colors.Neutral;
            SubFaction = SubFaction.Vampires;
            LastKill = DateTime.UtcNow;
            RoleType = RoleEnum.Dampyr;
            Faction = Faction.Neutral;
            FactionName = "Neutral";
            FactionColor = Colors.Neutral;
            RoleAlignment = RoleAlignment.NeutralPros;
            AlignmentName = () => "Neutral (Proselyte)";
            IntroText = "Kill off the Crew and help the Dracula";
            Results = InspResults.MineMafiSideDamp;
            AddToRoleHistory(RoleType);
        }

        public override void Wins()
        {
            DampWins = true;
        }

        public override void Loses()
        {
            LostByRPC = true;
        }

        public float KillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastKill;
            var num = CustomGameOptions.MurdKCD * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}