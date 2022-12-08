using TownOfUsReworked.Patches;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Modifiers.Modifiers
{
    public class Drunk : Modifier
    {
        public Drunk(PlayerControl player) : base(player)
        {
            Name = "Drunk";
            TaskText = "Inverrrrrted contrrrrols";
            Color = CustomGameOptions.CustomModifierColors ? Colors.Drunk : Colors.Modifier;
            ModifierType = ModifierEnum.Drunk;
            AddToModifierHistory(ModifierType);
        }
    }
}