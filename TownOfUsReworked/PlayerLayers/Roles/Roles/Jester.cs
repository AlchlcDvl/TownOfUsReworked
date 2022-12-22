using Il2CppSystem.Collections.Generic;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
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
            RoleType = RoleEnum.Jester;
            Faction = Faction.Neutral;
            FactionName = "Neutral";
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

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var jestTeam = new List<PlayerControl>();
            jestTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = jestTeam;
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