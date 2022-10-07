using TownOfUs.ImpostorRoles.UnderdogMod;

namespace TownOfUs.Roles
{
    public class Underdog : Role
    {
        public Underdog(PlayerControl player) : base(player)
        {
            Name = "Underdog";
            ImpostorText = () => "Use Your Comeback Power To Win";
            TaskText = () => PerformKill.LastImp()
                ? "You have a shortened kill cooldown!"
                : "You have a long kill cooldown until you're alone";
            if (CustomGameOptions.CustomImpColors) Color = Patches.Colors.Underdog;
            else Color = Patches.Colors.Impostor;
            RoleType = RoleEnum.Underdog;
            Faction = Faction.Intruders;
            FactionName = "Intruder";
            FactionColor = Patches.Colors.Impostor;
            Alignment = Alignment.IntruderKill;
            AlignmentName = "Intruder (Killing)";
            AddToRoleHistory(RoleType);
        }

        public float MaxTimer() => PerformKill.LastImp() ? PlayerControl.GameOptions.KillCooldown - CustomGameOptions.UnderdogKillBonus : (PerformKill.IncreasedKC() ? PlayerControl.GameOptions.KillCooldown : PlayerControl.GameOptions.KillCooldown + CustomGameOptions.UnderdogKillBonus);

        public void SetKillTimer()
        {
            Player.SetKillTimer(MaxTimer());
        }
    }
}
