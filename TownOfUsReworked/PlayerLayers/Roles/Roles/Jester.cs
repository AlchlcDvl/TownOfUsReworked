using Il2CppSystem.Collections.Generic;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Patches;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Jester : Role
    {
        public bool VotedOut;

        public Jester(PlayerControl player) : base(player)
        {
            Name = "Jester";
            StartText = "It Was Jest A Prank Bro";
            AbilitiesText = "Get ejected!\nFake Tasks:";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Jester : Colors.Neutral;
            SubFaction = SubFaction.None;
            RoleType = RoleEnum.Jester;
            Faction = Faction.Neutral;
            FactionName = "Neutral";
            Base = false;
            IsRecruit = false;
            FactionColor = Colors.Neutral;
            RoleAlignment = RoleAlignment.NeutralEvil;
            AlignmentName = "Neutral (Evil)";
            IntroText = "Get ejected";
            Results = InspResults.JestJuggWWInv;
            Attack = AttackEnum.None;
            Defense = DefenseEnum.None;
            AttackString = "None";
            DefenseString = "None";
            IntroSound = null;
            AddToRoleHistory(RoleType);
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__32 __instance)
        {
            var jestTeam = new List<PlayerControl>();
            jestTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = jestTeam;
        }

        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (!VotedOut | !Player.Data.IsDead && !Player.Data.Disconnected)
                return true;
                
            Utils.EndGame();
            return false;
        }

        public override void Wins()
        {
            VotedOut = true;
        }

        public override void Loses()
        {
            LostByRPC = true;
        }
    }
}