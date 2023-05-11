using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Modifiers
{
    public class Giant : Modifier, IVisualAlteration
    {
        private static bool Chonk => CustomGameOptions.DwarfScale != 1;
        private static bool Snail => CustomGameOptions.DwarfSpeed != 1;
        private static string Text => Chonk && Snail ? "big and slow" : (Chonk ? "big" : (Snail ? "slow" : ""));

        public Giant(PlayerControl player) : base(player)
        {
            Name = !Chonk && !Snail ? "Useless" : (!Chonk ? "Sloth" : (Snail ? "Chonker" : "Giant"));
            TaskText = !Chonk && !Snail ? "- Why" : $"- You are tiny {Text}";
            Color = CustomGameOptions.CustomModifierColors ? Colors.Giant : Colors.Modifier;
            ModifierType = ModifierEnum.Giant;
            Type = LayerEnum.Giant;
        }

        public bool TryGetModifiedAppearance(out VisualAppearance appearance)
        {
            appearance = Player.GetAppearance();
            return true;
        }
    }
}