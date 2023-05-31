namespace TownOfUsReworked.PlayerLayers.Modifiers
{
    public class Yeller : Modifier
    {
        public Yeller(PlayerControl player) : base(player)
        {
            Name = "Yeller";
            TaskText = () => "- Everyone knows where you are";
            Color = CustomGameOptions.CustomModifierColors ? Colors.Yeller : Colors.Modifier;
            ModifierType = ModifierEnum.Yeller;
            Type = LayerEnum.Yeller;

            if (TownOfUsReworked.IsTest)
                Utils.LogSomething($"{Player.name} is {Name}");
        }
    }
}