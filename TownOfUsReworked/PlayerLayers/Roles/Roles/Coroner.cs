using System.Collections.Generic;
using System.Linq;
using GameObject = UnityEngine.Object;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Lobby.CustomOption;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Coroner : Role
    {
        public Dictionary<byte, ArrowBehaviour> BodyArrows = new Dictionary<byte, ArrowBehaviour>();
        public readonly List<GameObject> Buttons = new List<GameObject>();
        public List<byte> Reported = new List<byte>();
        public bool CrewWin;

        public Coroner(PlayerControl player) : base(player)
        {
            Name = "Coroner";
            ImpostorText = () => "Understand When, Where And How Kills Happen";
            TaskText = () => "You get information from the dead";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Coroner : Colors.Crew;
            RoleType = RoleEnum.Coroner;
            Faction = Faction.Crew;
            FactionName = "Crew";
            FactionColor = Colors.Crew;
            RoleAlignment = RoleAlignment.CrewInvest;
            AlignmentName = () => "Crew (Investigative)";
            IntroText = "Eject all <color=#FF0000FF>evildoers</color>";
            CoronerDeadReport = $"The scalpels and human matter on the body indicate that this body is another Coroner!";
            CoronerKillerReport = "";
            Results = InspResults.CoroJaniUTMed;
            SubFaction = SubFaction.None;
            AddToRoleHistory(RoleType);
        }

        public void DestroyArrow(byte targetPlayerId)
        {
            var arrow = BodyArrows.FirstOrDefault(x => x.Key == targetPlayerId);

            if (arrow.Value != null)
                GameObject.Destroy(arrow.Value);
            if (arrow.Value.gameObject != null)
                GameObject.Destroy(arrow.Value.gameObject);
                
            BodyArrows.Remove(arrow.Key);
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var team = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            team.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = team;
        }

        public void Wins()
        {
            CrewWin = true;
        }

        public void Loses()
        {
            LostByRPC = true;
        }

        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (Player.Data.IsDead | Player.Data.Disconnected)
                return true;

            if ((PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Data.IsImpostor() |
                x.Is(RoleAlignment.NeutralKill) | x.Is(RoleAlignment.NeutralNeo) | x.Is(Faction.Syndicate) | x.Is(RoleAlignment.NeutralPros))) ==
                0) | (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.Disconnected && x.Is(Faction.Crew) && !x.Is(ObjectifierEnum.Lovers)
                && !x.Data.TasksDone()) == 0))
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