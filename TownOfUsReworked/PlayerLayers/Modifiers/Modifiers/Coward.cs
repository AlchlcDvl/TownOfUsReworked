using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Modifiers
{
    public class Coward : Modifier
    {
        public Coward(PlayerControl player) : base(player)
        {
            Name = "Coward";
            TaskText = "- You can't report bodies.";
            Color = CustomGameOptions.CustomModifierColors ? Colors.Coward : Colors.Modifier;
            ModifierType = ModifierEnum.Coward;
        }
    }
}