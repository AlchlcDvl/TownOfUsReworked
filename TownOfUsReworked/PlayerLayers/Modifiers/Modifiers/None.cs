namespace TownOfUsReworked.PlayerLayers.Modifiers
{
    public class Modifierless : Modifier
    {
        public override bool Hidden => true;

        public Modifierless(PlayerControl player) : base(player) {}
    }
}