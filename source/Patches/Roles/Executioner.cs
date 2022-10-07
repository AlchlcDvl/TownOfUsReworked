using Il2CppSystem.Collections.Generic;

namespace TownOfUs.Roles
{
    public class Executioner : Role
    {
        public PlayerControl target;
        public bool TargetVotedOut;

        public Executioner(PlayerControl player) : base(player)
        {
            Name = "Executioner";
            ImpostorText = () =>
                target == null
                ? "You don't have a target for some reason... weird..."
                : $"Eject {target.name}";
            TaskText = () =>
                target == null
                    ? "You don't have a target for some reason... weird..."
                    : $"Eject {target.name}!\nFake Tasks:";
            if (CustomGameOptions.CustomNeutColors) Color = Patches.Colors.Executioner;
            else Color = Patches.Colors.Neutral;
            RoleType = RoleEnum.Executioner;
            Faction = Faction.Neutral;
            Scale = 1.4f;
            FactionName = "Neutral";
            FactionColor = Patches.Colors.Neutral;
            Alignment = Alignment.NeutralEvil;
            AlignmentName = "Neutral (Evil)";
            AddToRoleHistory(RoleType);
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var exeTeam = new List<PlayerControl>();
            exeTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = exeTeam;
        }

        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (Player.Data.IsDead) return true;
            if (!TargetVotedOut || !target.Data.IsDead) return true;
            Utils.EndGame();
            return false;
        }

        public void Wins()
        {
            if (Player.Data.IsDead || Player.Data.Disconnected) return;
            TargetVotedOut = true;
        }

        public void Loses()
        {
            LostByRPC = true;
        }
    }
}