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
        public Dictionary<byte, ArrowBehaviour> BodyArrows;
        public DeadBody CurrentTarget = null;
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
            //IntroSound = "AmnesiacIntro";
            RoleDescription = "Your are an Amnesiac! You know when players die and need to find a dead player. You cannot win as your current role and" +
                " instead need to win as the role you become after finding a dead body.";
            Objectives = "- Find a dead body, remember their role and then fulfill the win condition for that role.";
            BodyArrows = new Dictionary<byte, ArrowBehaviour>();
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