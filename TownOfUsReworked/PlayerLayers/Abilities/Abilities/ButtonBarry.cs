using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Patches;

namespace TownOfUsReworked.PlayerLayers.Abilities.Abilities
{
    public class ButtonBarry : Ability
    {
        public KillButton ButtonButton;
        public bool ButtonUsed;

        public ButtonBarry(PlayerControl player) : base(player)
        {
            Name = "Button Barry";
            TaskText = "Call a button from anywhere!";
            Color = CustomGameOptions.CustomAbilityColors ? Colors.ButtonBarry : Colors.Ability;
            AbilityType = AbilityEnum.ButtonBarry;
            AddToAbilityHistory(AbilityType);
        }
    }
}