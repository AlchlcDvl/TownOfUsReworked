using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Modifiers
{
    public class Drunk : Modifier
    {
        public Drunk(PlayerControl player) : base(player)
        {
            Name = "Drunk";
            TaskText = "- Your controls are inverted.";
            Color = CustomGameOptions.CustomModifierColors ? Colors.Drunk : Colors.Modifier;
            ModifierType = ModifierEnum.Drunk;
        }
    }
}