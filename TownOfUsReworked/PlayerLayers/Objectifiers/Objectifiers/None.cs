namespace TownOfUsReworked.PlayerLayers.Objectifiers
{
    public class Objectifierless : Objectifier
    {
        public Objectifierless(PlayerControl player) : base(player)
        {
            Name = "None";
            Hidden = true;
            Type = LayerEnum.None;

            if (TownOfUsReworked.IsTest)
                Utils.LogSomething($"{Player.name} is {Name}");
        }
    }
}