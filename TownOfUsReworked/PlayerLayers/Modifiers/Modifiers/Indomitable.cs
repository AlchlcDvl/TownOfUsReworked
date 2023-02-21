using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Modifiers.Modifiers
{
    public class Indomitable : Modifier
    {
        public Indomitable(PlayerControl player) : base(player)
        {
            Name = "Indomitable";
            TaskText = "You're unguessable";
            Color = CustomGameOptions.CustomModifierColors ? Colors.Indomitable : Colors.Modifier;
            ModifierType = ModifierEnum.Indomitable;
        }
    }
}