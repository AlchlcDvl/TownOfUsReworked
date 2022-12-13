using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Patches;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Amnesiac : Role
    {
        public Dictionary<byte, ArrowBehaviour> BodyArrows = new Dictionary<byte, ArrowBehaviour>();
        public DeadBody CurrentTarget;

        public Amnesiac(PlayerControl player) : base(player)
        {
            Name = "Amnesiac";
            StartText = "You forgor :skull:";
            AbilitiesText = "- You can get a player's role should you find their body.";
            AttributesText = "- You do not win as a regular Amnesiac without remebering.";
            RoleType = RoleEnum.Amnesiac;
            Faction = Faction.Neutral;
            FactionName = "Neutral";
            FactionColor = Colors.Neutral;
            RoleAlignment = RoleAlignment.NeutralBen;
            AlignmentName = "Neutral (Benign)";
            IntroText = "Remember a dead person";
            CoronerDeadReport = "There are signs of blunt force trauma to the head. They must be an Amnesiac!";
            CoronerKillerReport = "";
            Results = InspResults.EngiAmneThiefCann;
            Color = CustomGameOptions.CustomNeutColors ? Colors.Amnesiac : Colors.Neutral;
            SubFaction = SubFaction.None;
            IntroSound = TownOfUsReworked.AmnesiacIntro;
            RoleDescription = "Your are an Amnesiac! You know when players die and need to find a dead player. You cannot win as your current role and" +
                " instead need to win as the role you become after finding a dead body.";
            AlignmentDescription = "You are a Neutral (Benign) role! You can win with anyone as long as a certain condition has been fulfilled for you.";
            Objectives = "- Find a dead body, remember their role and then fulfill the win condition for that role.";
            Attack = AttackEnum.None;
            Defense = DefenseEnum.None;
            AttackString = "None";
            DefenseString = "None";
            FactionDescription = "Your faction is Neutral! You do not have any team mates and can only by yourself or by other players after finishing" +
                " a certain objective.";
            AddToRoleHistory(RoleType);
        }

        public override void Loses()
        {
            LostByRPC = true;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__32 __instance)
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