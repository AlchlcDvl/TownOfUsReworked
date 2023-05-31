namespace TownOfUsReworked.PlayerLayers.Modifiers
{
    public class Flincher : Modifier
    {
        public Flincher(PlayerControl player) : base(player)
        {
            Name = "Flincher";
            TaskText = () => "- You will randomly flinch while walking";
            Color = CustomGameOptions.CustomModifierColors ? Colors.Flincher : Colors.Modifier;
            ModifierType = ModifierEnum.Flincher;
            Type = LayerEnum.Flincher;

            if (TownOfUsReworked.IsTest)
                Utils.LogSomething($"{Player.name} is {Name}");
        }
    }
}