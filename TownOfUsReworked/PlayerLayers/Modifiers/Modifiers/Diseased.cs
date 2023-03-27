using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Functions;

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