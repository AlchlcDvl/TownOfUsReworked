namespace TownOfUsReworked.PlayerLayers.Objectifiers
{
    public class Mafia : Objectifier
    {
        public Mafia(PlayerControl player) : base(player)
        {
            Name = "Mafia";
            Symbol = "ω";
            TaskText = "- Eliminate anyone who opposes the Mafia";
            Color = CustomGameOptions.CustomObjectifierColors ? Colors.Mafia : Colors.Objectifier;
            ObjectifierType = ObjectifierEnum.Mafia;
            Type = LayerEnum.Mafia;
            Role.GetRole(Player).RoleAlignment = Role.GetRole(Player).RoleAlignment.GetNewAlignment(Faction.Neutral);
        }
    }
}