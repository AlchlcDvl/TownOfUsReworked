using Il2CppSystem.Collections.Generic;

namespace TownOfUs.Roles
{
    public class Jester : Role
    {
        public bool VotedOut;

        public Jester(PlayerControl player) : base(player)
        {
            Name = "Jester";
            ImpostorText = () => "It Was Jest A Prank Bro";
            TaskText = () => "Get ejected!\nFake Tasks:";
            if (CustomGameOptions.CustomNeutColors) Color = Patches.Colors.Jester;
            else Color = Patches.Colors.Neutral;
            RoleType = RoleEnum.Jester;
            Faction = Faction.Neutral;
            FactionName = "Neutral";
            FactionColor = Patches.Colors.Neutral;
            Alignment = Alignment.NeutralEvil;
            AlignmentName = "Neutral (Evil)";
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
            if (!VotedOut || !Player.Data.IsDead && !Player.Data.Disconnected) return true;
            Utils.EndGame();
            return false;
        }

        public void Wins()
        {
            //System.Console.WriteLine("Reached Here - Jester edition");
            VotedOut = true;
        }

        public void Loses()
        {
            LostByRPC = true;
        }
    }
}