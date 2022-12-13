using Il2CppSystem.Collections.Generic;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Patches;
using System;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Troll : Role
    {
        public bool Killed;
        public DateTime LastInteracted { get; set; }
        public PlayerControl ClosestPlayer;

        public Troll(PlayerControl player) : base(player)
        {
            Name = "Troll";
            StartText = "Troll Everyone With Your Death";
            AbilitiesText = "Die!\nFake Tasks:";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Troll : Colors.Neutral;
            SubFaction = SubFaction.None;
            RoleType = RoleEnum.Troll;
            Faction = Faction.Neutral;
            FactionName = "Neutral";
            FactionColor = Colors.Neutral;
            RoleAlignment = RoleAlignment.NeutralEvil;
            AlignmentName = "Neutral (Evil)";
            IntroText = "Die";
            Results = InspResults.ArsoCryoPBOpTroll;
            LastInteracted = DateTime.UtcNow;
            IntroSound = null;
            Attack = AttackEnum.None;
            Defense = DefenseEnum.None;
            AttackString = "None";
            DefenseString = "None";
            AddToRoleHistory(RoleType);
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__32 __instance)
        {
            var Team = new List<PlayerControl>();
            Team.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = Team;
        }

        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (!Killed | (!Player.Data.IsDead && !Player.Data.Disconnected))
                return true;
                
            Utils.EndGame();
            return false;
        }

        public override void Wins()
        {
            Killed = true;
        }

        public override void Loses()
        {
            LostByRPC = true;
        }

        public bool KilledFunc()
        {
            return true;
        }

        public float InteractTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastInteracted;
            var num = CustomGameOptions.InfectCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}