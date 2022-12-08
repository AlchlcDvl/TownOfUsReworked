using TownOfUsReworked.Patches;
using TownOfUsReworked.Lobby.CustomOption;
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
            AddToModifierHistory(ModifierType);
        }
    }
}