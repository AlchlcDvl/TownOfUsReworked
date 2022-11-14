using TownOfUsReworked.Patches;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Modifiers.DrunkMod
{
    public class Drunk : Modifier
    {
        public Drunk(PlayerControl player) : base(player)
        {
            Name = "Drunk";
            TaskText = () => "Inverrrrrted contrrrrols";
            if (CustomGameOptions.CustomModifierColors) Color = Colors.Drunk;
            else Color = Colors.Modifier;
            ModifierType = ModifierEnum.Drunk;
            AddToModifierHistory(ModifierType);
        }
    }
}