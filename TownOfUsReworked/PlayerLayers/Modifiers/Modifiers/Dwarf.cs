namespace TownOfUsReworked.PlayerLayers.Modifiers
{
    public class Dwarf : Modifier
    {
        private static bool Smol => CustomGameOptions.DwarfScale != 1;
        private static bool Sped => CustomGameOptions.DwarfSpeed != 1;
        private static string Text => Smol && Sped ? "tiny and speedy" : (Smol ? "tiny" : (Sped ? "speedy" : ""));

        public Dwarf(PlayerControl player) : base(player)
        {
            Name = !Smol && !Sped ? "Useless" : (!Smol ? "Flash" : (Sped ? "Gremlin" : "Dwarf"));
            TaskText = () => !Smol && !Sped ? "- Why" : $"- You are tiny {Text}";
            Color = CustomGameOptions.CustomModifierColors ? Colors.Dwarf : Colors.Modifier;
            ModifierType = ModifierEnum.Dwarf;
            Type = LayerEnum.Dwarf;

            if (TownOfUsReworked.IsTest)
                Utils.LogSomething($"{Player.name} is {Name}");
        }
    }
}