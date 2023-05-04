using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Modifiers
{
    public class Dwarf : Modifier, IVisualAlteration
    {
        public Dwarf(PlayerControl player) : base(player)
        {
            var fastText = CustomGameOptions.DwarfSpeed != 1 ? " with speed" : "";
            Name = "Dwarf";
            TaskText = $"- You are tiny{fastText}.";
            Color = CustomGameOptions.CustomModifierColors ? Colors.Dwarf : Colors.Modifier;
            ModifierType = ModifierEnum.Dwarf;
            Type = LayerEnum.Dwarf;
        }

        public bool TryGetModifiedAppearance(out VisualAppearance appearance)
        {
            appearance = Player.GetDefaultAppearance();
            appearance.SpeedFactor *= CustomGameOptions.DwarfSpeed;
            appearance.SizeFactor *= CustomGameOptions.DwarfScale;
            return true;
        }
    }
}