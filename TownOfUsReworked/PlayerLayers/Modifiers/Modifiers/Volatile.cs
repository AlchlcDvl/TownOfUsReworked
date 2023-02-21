using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Modifiers.Modifiers
{
    public class Volatile : Modifier
    {
        public Volatile(PlayerControl player) : base(player)
        {
            Name = "Volatile";
            TaskText = "You might see/hear things and lash out.";
            Color = CustomGameOptions.CustomModifierColors ? Colors.Volatile : Colors.Modifier;
            ModifierType = ModifierEnum.Volatile;
            Hidden = !CustomGameOptions.VolatileKnows;
        }
    }
}