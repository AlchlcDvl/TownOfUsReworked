namespace TownOfUsReworked.PlayerLayers.Modifiers
{
    public class Modifierless : Modifier
    {
        public Modifierless(PlayerControl player) : base(player)
        {
            Name = "None";
            Hidden = true;
        }
    }
}