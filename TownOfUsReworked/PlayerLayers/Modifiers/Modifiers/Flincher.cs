using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Modifiers
{
    public class Flincher : Modifier
    {
        public Flincher(PlayerControl player) : base(player)
        {
            Name = "Flincher";
            TaskText = "- You will randomly flinch while walking.";
            Color = CustomGameOptions.CustomModifierColors ? Colors.Flincher : Colors.Modifier;
            Type = ModifierEnum.Flincher;
        }
    }
}