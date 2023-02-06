using TownOfUsReworked.Classes;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Modifiers.Modifiers
{
    public class Coward : Modifier
    {
        public Coward(PlayerControl player) : base(player)
        {
            Name = "Coward";
            TaskText = "You are too afraid to report bodies";
            Color = CustomGameOptions.CustomModifierColors ? Colors.Coward : Colors.Modifier;
            ModifierType = ModifierEnum.Coward;
        }
    }
}