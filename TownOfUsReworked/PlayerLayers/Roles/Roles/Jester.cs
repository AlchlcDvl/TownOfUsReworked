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
            ImpostorText = () => "It Was Jest A Prank Bro";
            TaskText = () => "Get ejected!\nFake Tasks:";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Jester : Colors.Neutral;
            SubFaction = SubFaction.None;
            RoleType = RoleEnum.Jester;
            Faction = Faction.Neutral;
            FactionName = "Neutral";
            FactionColor = Colors.Neutral;
            RoleAlignment = RoleAlignment.NeutralEvil;
            AlignmentName = () => "Neutral (Evil)";
            IntroText = "Get ejected";
            Results = InspResults.JestJuggWWInv;
            AddToRoleHistory(RoleType);
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
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

        public void Wins()
        {
            VotedOut = true;
        }

        public void Loses()
        {
            LostByRPC = true;
        }
    }
}