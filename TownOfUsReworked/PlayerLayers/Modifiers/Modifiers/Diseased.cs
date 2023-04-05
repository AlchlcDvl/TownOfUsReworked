using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Modifiers
{
    public class Diseased : Modifier
    {
        public Diseased(PlayerControl player) : base(player)
        {
            Name = "Diseased";
            TaskText = $"- Your killer's cooldown increases by {CustomGameOptions.DiseasedMultiplier} times.";
            Color = CustomGameOptions.CustomModifierColors ? Colors.Diseased : Colors.Modifier;
            ModifierType = ModifierEnum.Diseased;
            Hidden = !CustomGameOptions.DiseasedKnows;
        }
    }
}