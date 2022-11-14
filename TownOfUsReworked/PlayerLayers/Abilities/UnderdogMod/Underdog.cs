using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Patches;

namespace TownOfUsReworked.PlayerLayers.Abilities.UnderdogMod
{
    public class Underdog : Ability
    {
        public Underdog(PlayerControl player) : base(player)
        {
            Name = "Underdog";
            TaskText = () => Utils.LastImp()
                ? "You have a shortened kill cooldown!"
                : "You have a long kill cooldown until you're alone";
            if (CustomGameOptions.CustomAbilityColors) Color = Colors.Underdog;
            else Color = Colors.Ability;
            AbilityType = AbilityEnum.Underdog;
            AddToAbilityHistory(AbilityType);
        }

        public float MaxTimer() => Utils.LastImp() ? PlayerControl.GameOptions.KillCooldown - CustomGameOptions.UnderdogKillBonus :
            (PerformKill.IncreasedKC() ? PlayerControl.GameOptions.KillCooldown : PlayerControl.GameOptions.KillCooldown +
            CustomGameOptions.UnderdogKillBonus);

        public void SetKillTimer()
        {
            Player.SetKillTimer(MaxTimer());
        }
    }
}
