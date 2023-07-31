namespace TownOfUsReworked.PlayerLayers.Objectifiers
{
    public class Objectifierless : Objectifier
    {
        public override bool Hidden => true;

        public Objectifierless(PlayerControl player) : base(player) {}
    }
}