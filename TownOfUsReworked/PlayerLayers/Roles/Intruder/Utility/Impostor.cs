namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Impostor : IntruderRole
    {
        public Impostor(PlayerControl player) : base(player)
        {
            Name = "Impostor";
            RoleType = RoleEnum.Impostor;
            StartText = "Sabotage And Kill Everyone";
            RoleAlignment = RoleAlignment.IntruderUtil;
            Base = true;
            Type = LayerEnum.Impostor;
            InspectorResults = InspectorResults.IsBasic;
        }
    }
}