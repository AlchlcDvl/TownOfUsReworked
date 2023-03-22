using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Modifiers
{
    public class Flincher : Modifier
    {
        public Flincher(PlayerControl player) : base(player)
        {
            Name = "Flincher";
            TaskText = "- You will randomly flinch while walking.";
            Color = CustomGameOptions.CustomModifierColors ? Colors.Flincher : Colors.Modifier;
            ModifierType = ModifierEnum.Flincher;
        }
    }
}