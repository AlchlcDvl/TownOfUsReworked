using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Patches;
using TownOfUsReworked.PlayerLayers.Abilities.UnderdogMod;

namespace TownOfUsReworked.PlayerLayers.Abilities.Abilities
{
    public class Underdog : Ability
    {
        public float MaxTimer => Utils.LastImp() ? PlayerControl.GameOptions.KillCooldown - CustomGameOptions.UnderdogKillBonus :
            (PerformKill.IncreasedKC() ? PlayerControl.GameOptions.KillCooldown + CustomGameOptions.UnderdogKillBonus :
            PlayerControl.GameOptions.KillCooldown);

        public Underdog(PlayerControl player) : base(player)
        {
            Name = "Underdog";
            TaskText = () => Utils.LastImp()
                ? "You have a shortened kill cooldown!"
                : "You have a long kill cooldown until you're alone";
            Color = CustomGameOptions.CustomAbilityColors ? Colors.Underdog : Colors.Ability;
            AbilityType = AbilityEnum.Underdog;
            AddToAbilityHistory(AbilityType);
        }

        public void SetKillTimer()
        {
            Player.SetKillTimer(MaxTimer);
        }
    }
}
