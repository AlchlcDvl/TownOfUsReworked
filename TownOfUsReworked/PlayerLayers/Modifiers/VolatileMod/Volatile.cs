using TownOfUsReworked.Patches;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Modifiers.VolatileMod
{
    public class Volatile : Modifier
    {
        public Volatile(PlayerControl player) : base(player)
        {
            Name = "Volatile";
            TaskText = () => "You might see/hear things and lash out.";
            if (CustomGameOptions.CustomModifierColors) Color = Colors.Volatile;
            else Color = Colors.Modifier;
            ModifierType = ModifierEnum.Volatile;
            AddToModifierHistory(ModifierType);
        }
    }
}