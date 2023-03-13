using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Altruist : CrewRole
    {
        public bool CurrentlyReviving = false;
        public DeadBody CurrentTarget = null;
        public bool ReviveUsed = false;
        public AbilityButton ReviveButton;
        
        public Altruist(PlayerControl player) : base(player)
        {
            Name = "Altruist";
            StartText = "Sacrifice Yourself To Save Another";
            AbilitiesText = $"- You can revive a dead body at the cost of your own life.\n- Reviving someone takes {CustomGameOptions.AltReviveDuration}s.\n- If a meeting is called " +
                "during your revive, both you and\nyour target will be pronounced dead.";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Altruist : Colors.Crew;
            RoleType = RoleEnum.Altruist;
            RoleAlignment = RoleAlignment.CrewProt;
            AlignmentName = CP;
            RoleDescription = "Your are an Altruist! You can revive a dead person if you find their body. Be careful though, because it takes time" +
                " to revive someone and a meeting being called will kill both you and your target.";
            InspectorResults = InspectorResults.MeddlesWithDead;
        }
    }
}