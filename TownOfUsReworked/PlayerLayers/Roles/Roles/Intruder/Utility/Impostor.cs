using TownOfUsReworked.Enums;

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
            //IntroSound = TownOfUsReworked.ImpostorIntro;
            Base = true;
            RoleDescription = "You are an Impostor! Your role is the base role for the Intruder faction. You have no special abilities and should probably just kill normally.";
        }
    }
}