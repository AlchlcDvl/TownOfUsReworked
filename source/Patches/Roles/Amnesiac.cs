using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;

namespace TownOfUs.Roles
{
    public class Amnesiac : Role
    {
        public Dictionary<byte, ArrowBehaviour> BodyArrows = new Dictionary<byte, ArrowBehaviour>();

        public Amnesiac(PlayerControl player) : base(player)
        {
            Name = "Amnesiac";
            ImpostorText = () => "You forgor :skull:";
            TaskText = () => "Find a body to steal their role";
            if (CustomGameOptions.CustomNeutColors) Color = Patches.Colors.Amnesiac;
            else Color = Patches.Colors.Neutral;
            RoleType = RoleEnum.Amnesiac;
            Faction = Faction.Neutral;
            FactionName = "Neutral";
            FactionColor = Patches.Colors.Neutral;
            Alignment = Alignment.NeutralBen;
            AlignmentName = "Neutral (Benign)";
            AddToRoleHistory(RoleType);
        }

        public DeadBody CurrentTarget;

        public void Loses()
        {
            LostByRPC = true;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var amnesiacTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            amnesiacTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = amnesiacTeam;
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