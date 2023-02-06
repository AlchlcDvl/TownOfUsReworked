using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Patches;
using TownOfUsReworked.PlayerLayers.Abilities.UnderdogMod;

namespace TownOfUsReworked.PlayerLayers.Abilities.Abilities
{
    public class Underdog : Ability
    {
        public float MaxTimer => Player.Is(Faction.Intruder) ? (Utils.LastImp() ? CustomGameOptions.IntKillCooldown - CustomGameOptions.UnderdogKillBonus :
            (PerformKill.IncreasedKC() ? CustomGameOptions.IntKillCooldown + CustomGameOptions.UnderdogKillBonus :
            CustomGameOptions.IntKillCooldown)) : (Utils.LastSyn() ? CustomGameOptions.ChaosDriveKillCooldown - CustomGameOptions.UnderdogKillBonus :
            (PerformKill.IncreasedKC() ? CustomGameOptions.ChaosDriveKillCooldown + CustomGameOptions.UnderdogKillBonus :
            CustomGameOptions.ChaosDriveKillCooldown));

        public Underdog(PlayerControl player) : base(player)
        {
            Name = "Underdog";
            TaskText = Utils.LastImp()
                ? "You have a shortened kill cooldown!"
                : "You have a long kill cooldown until you're alone";
            Color = CustomGameOptions.CustomAbilityColors ? Colors.Underdog : Colors.Ability;
            AbilityType = AbilityEnum.Underdog;
            Hidden = !CustomGameOptions.UnderdogKnows;
        }

        public void SetKillTimer()
        {
            Player.SetKillTimer(MaxTimer);
        }
    }
}
