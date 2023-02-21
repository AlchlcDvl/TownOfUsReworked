using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Modifiers.Modifiers
{
    public class Drunk : Modifier
    {
        public Drunk(PlayerControl player) : base(player)
        {
            Name = "Drunk";
            TaskText = "Inverrrrrted contrrrrols";
            Color = CustomGameOptions.CustomModifierColors ? Colors.Drunk : Colors.Modifier;
            ModifierType = ModifierEnum.Drunk;
        }
    }
}