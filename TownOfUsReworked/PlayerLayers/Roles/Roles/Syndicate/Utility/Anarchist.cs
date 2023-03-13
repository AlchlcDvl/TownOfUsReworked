using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Anarchist : SyndicateRole
    {
        public Anarchist(PlayerControl player) : base(player)
        {
            Name = "Anarchist";
            RoleType = RoleEnum.Anarchist;
            StartText = "Wreck Everyone With A Passion";
            RoleAlignment = RoleAlignment.SyndicateUtil;
            AlignmentName = SU;
            Base = true;
            RoleDescription = "You are an Anarchist! Your role is the base role for the Syndicate faction. You have no special abilities aside from being able to kill.";
        }
    }
}