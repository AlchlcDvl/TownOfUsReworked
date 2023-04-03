using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Anarchist : SyndicateRole
    {
        public Anarchist(PlayerControl player) : base(player)
        {
            Name = "Anarchist";
            Type = RoleEnum.Anarchist;
            StartText = "Wreck Everyone With A Passion";
            RoleAlignment = RoleAlignment.SyndicateUtil;
            AlignmentName = SU;
            Base = true;
        }
    }
}