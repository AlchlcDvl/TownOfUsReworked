using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Lobby.CustomOption;
using Il2CppSystem.Collections.Generic;
using TownOfUsReworked.Extensions;
using Hazel;
using System.Linq;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Altruist : Role
    {
        public bool CurrentlyReviving;
        public DeadBody CurrentTarget;
        public bool ReviveUsed;
        
        public Altruist(PlayerControl player) : base(player)
        {
            Name = "Altruist";
            StartText = "Sacrifice Yourself To Save Another";
            AbilitiesText = "- You can revive a dead body at the cost of your own life.\n- Reviving someone takes time.";
            AttributesText = "- If a meeting is called during your revive, both you can your target will be pronounced dead.";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Altruist : Colors.Crew;
            RoleType = RoleEnum.Altruist;
            Faction = Faction.Crew;
            FactionName = "Crew";
            FactionColor = Colors.Crew;
            RoleAlignment = RoleAlignment.CrewProt;
            AlignmentName = "Crew (Protective)";
            IntroText = "Eject all <color=#FF0000FF>evildoers</color>";
            CoronerDeadReport = "The chemicals and health items indicate that this body is an Altruist! You probably should not have reported it.";
            CoronerKillerReport = "";
            Results = InspResults.TrackAltTLTM;
            SubFaction = SubFaction.None;
            IntroSound = null;
            Attack = AttackEnum.None;
            AttackString = "None";
            Defense = DefenseEnum.None;
            DefenseString = "None";
            FactionDescription = "Your faction is the Crew! You do not know who the other members of your faction are. It is your job to deduce" + 
                " who is evil and who is not. Eject or kill all evils or finish all of your tasks to win!";
            AlignmentDescription = "You are a Crew (Protective) role! You have the capability to stop someone from losing their life, and quite possibly" +
                " even gain information from the dead!";
            RoleDescription = "Your are an Altruist! You can revive a dead person if you find their body. Be careful though, because it takes time" +
                " to revive someone and a meeting being called will kill both you can the target.";
            Objectives = "- Finish your tasks along with other Crew.\n   or\n- Kill: <color=#FF0000FF>Intruders</color>, <color=#008000FF>Syndicate</color>" + 
                " and <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killers</color>, <color=#1D7CF2FF>Proselytes</color> and " +
                "<color=#1D7CF2FF>Neophytes</color>.";
            AddToRoleHistory(RoleType);
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var team = new List<PlayerControl>();
            team.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = team;
        }

        public override void Wins()
        {
            CrewWin = true;
        }

        public override void Loses()
        {
            LostByRPC = true;
        }

        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (Player.Data.IsDead | Player.Data.Disconnected)
                return true;

            if ((PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(Faction.Intruders) |
                x.Is(RoleAlignment.NeutralKill) | x.Is(RoleAlignment.NeutralNeo) | x.Is(Faction.Syndicate) | x.Is(RoleAlignment.NeutralPros))) ==
                0) | Utils.TasksDone())
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CrewWin,
                    SendOption.Reliable, -1);
                writer.Write(Player.PlayerId);
                Wins();
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }

            return false;
        }
    }
}