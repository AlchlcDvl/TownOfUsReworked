namespace TownOfUsReworked.PlayerLayers.Objectifiers
{
    public class Mafia : Objectifier
    {
        public Mafia(PlayerControl player) : base(player)
        {
            Name = "Mafia";
            SymbolName = "ω";
            TaskText = "- Eliminate anyone who opposes the Mafia";
            Color = CustomGameOptions.CustomObjectifierColors ? Colors.Mafia : Colors.Objectifier;
            ObjectifierType = ObjectifierEnum.Mafia;
            Type = LayerEnum.Mafia;
        }
    }
}