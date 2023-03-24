using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Amnesiac : NeutralRole
    {
        public Dictionary<byte, ArrowBehaviour> BodyArrows = new();
        public DeadBody CurrentTarget;
        public AbilityButton RememberButton;

        public Amnesiac(PlayerControl player) : base(player)
        {
            Name = "Amnesiac";
            StartText = "You Forgor :Skull:";
            AbilitiesText = "- You can copy over a player's role should you find their body." + (CustomGameOptions.RememberArrows ? "\n- When someone dies, you get an arrow " +
                "pointing to their body." : "");
            RoleType = RoleEnum.Amnesiac;
            RoleAlignment = RoleAlignment.NeutralBen;
            AlignmentName = NB;
            Color = CustomGameOptions.CustomNeutColors ? Colors.Amnesiac : Colors.Neutral;
            Objectives = "- Find a dead body, remember their role and then fulfill the win condition for that role.";
            BodyArrows = new();
            InspectorResults = InspectorResults.DealsWithDead;
        }

        public void DestroyArrow(byte targetPlayerId)
        {
            var arrow = BodyArrows.FirstOrDefault(x => x.Key == targetPlayerId);

            if (arrow.Value != null)
                Object.Destroy(arrow.Value);
            if (arrow.Value.gameObject != null)
                Object.Destroy(arrow.Value.gameObject);

            BodyArrows.Remove(arrow.Key);
        }
    }
}