using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Enums;

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
            ModifierDescription = "You are Diseased! When you are killed, you pass on your sickness to your killer, causing them to have an increased cooldown!";
        }
    }
}