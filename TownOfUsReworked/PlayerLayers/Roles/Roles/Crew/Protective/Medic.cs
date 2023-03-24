using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Medic : CrewRole
    {
        public PlayerControl ClosestPlayer;
        public bool UsedAbility => ShieldedPlayer != null || ExShielded != null;
        public PlayerControl ShieldedPlayer;
        public PlayerControl ExShielded;
        public AbilityButton ShieldButton;

        public Medic(PlayerControl player) : base(player)
        {
            Name = "Medic";
            StartText = "Shield A Player To Protect Them";
            AbilitiesText = "- You can shield a player to prevent them from dying to others.\n- If your target is attacked, you will be notified of it by default.\n- Your shield does " +
                "not save your target from suicides.";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Medic : Colors.Crew;
            RoleType = RoleEnum.Medic;
            ShieldedPlayer = null;
            ExShielded = null;
            RoleAlignment = RoleAlignment.CrewProt;
            AlignmentName = CP;
            InspectorResults = InspectorResults.SeeksToProtect;
        }
    }
}