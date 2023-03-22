using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Modifiers
{
    public class Indomitable : Modifier
    {
        public Indomitable(PlayerControl player) : base(player)
        {
            Name = "Indomitable";
            TaskText = "- You cannot be guessed.";
            Color = CustomGameOptions.CustomModifierColors ? Colors.Indomitable : Colors.Modifier;
            ModifierType = ModifierEnum.Indomitable;
            Hidden = !CustomGameOptions.IndomitableKnows;
        }
    }
}