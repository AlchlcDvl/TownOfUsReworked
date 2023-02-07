using System;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Lobby.CustomOption;
using Il2CppSystem.Collections.Generic;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Thief : Role
    {
        public PlayerControl ClosestPlayer;
        public DateTime LastKilled { get; set; }
        private KillButton _stealButton;

        public Thief(PlayerControl player) : base(player)
        {
            Name = "Thief";
            StartText = "Steal From The Killers";
            AbilitiesText = "Steal From The Killers";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Thief : Colors.Neutral;
            LastKilled = DateTime.UtcNow;
            RoleType = RoleEnum.Thief;
            Faction = Faction.Neutral;
            FactionName = "Neutral";
            FactionColor = Colors.Neutral;
            RoleAlignment = RoleAlignment.NeutralBen;
            AlignmentName = "Neutral (Benign)";
            Results = InspResults.ThiefAmneTrackInvest;
        }

        public override void Loses()
        {
            LostByRPC = true;
        }

        public KillButton StealButton
        {
            get => _stealButton;
            set
            {
                _stealButton = value;
                AddToAbilityButtons(value, this);
            }
        }

        public float KillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastKilled;
            var num = CustomGameOptions.ThiefKillCooldown * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__32 __instance)
        {
            if (Player != PlayerControl.LocalPlayer)
                return;
                
            var team = new List<PlayerControl>();
            team.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = team;
        }
    }
}