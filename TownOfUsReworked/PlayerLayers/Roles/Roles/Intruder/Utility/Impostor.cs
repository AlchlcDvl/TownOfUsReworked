using TownOfUsReworked.Data;

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
            AlignmentName = IU;
            Base = true;
        }
    }
}