namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Roleless : Role
    {
        public Roleless(PlayerControl player) : base(player)
        {
            Name = "None";
            InspectorResults = InspectorResults.None;
            Type = LayerEnum.None;
            Player.Data.SetImpostor(false);

            if (TownOfUsReworked.IsTest)
                Utils.LogSomething($"{Player.name} is {Name}");
        }
    }
}