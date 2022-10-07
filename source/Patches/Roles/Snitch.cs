using System.Collections.Generic;
using TownOfUs.Extensions;
using UnityEngine;
using System.Linq;

namespace TownOfUs.Roles
{
    public class Snitch : Role
    {
        public List<ArrowBehaviour> ImpArrows = new List<ArrowBehaviour>();
        public Dictionary<byte, ArrowBehaviour> SnitchArrows = new Dictionary<byte, ArrowBehaviour>();

        public Snitch(PlayerControl player) : base(player)
        {
            Name = "Snitch";
            ImpostorText = () => "Do Tasks To Reveal The <color=#FF0000FF>Intruders</color>";
            TaskText = () =>
                TasksDone
                    ? "Follow the arrows pointing to the <color=#FF0000FF>Intruders</color>!"
                    : "Complete all your tasks to discover the <color=#FF0000FF>Intruders</color>!";
            if (CustomGameOptions.CustomCrewColors) Color = Patches.Colors.Snitch;
            else Color = Patches.Colors.Crew;
            RoleType = RoleEnum.Snitch;
            Faction = Faction.Crewmates;
            FactionName = "Crew";
            FactionColor = Patches.Colors.Crew;
            Alignment = Alignment.CrewInvest;
            AlignmentName = "Crew (Investigative)";
            AddToRoleHistory(RoleType);
        }

        public bool Revealed => TasksLeft <= CustomGameOptions.SnitchTasksRemaining;
        public bool TasksDone => TasksLeft <= 0;

        internal override bool Criteria()
        {
            return Revealed && PlayerControl.LocalPlayer.Data.IsImpostor() && !Player.Data.IsDead || base.Criteria();
        }

        internal override bool RoleCriteria()
        {
            var localPlayer = PlayerControl.LocalPlayer;
            if (localPlayer.Data.IsImpostor() && !Player.Data.IsDead)
            {
                return Revealed || base.RoleCriteria();
            }
            else if (Role.GetRole(localPlayer).Faction == Faction.Neutral && !Player.Data.IsDead)
            {
                return Revealed && CustomGameOptions.SnitchSeesNeutrals || base.RoleCriteria();
            }
            return false || base.RoleCriteria();
        }
        
        protected override string NameText(bool revealTasks, bool revealRole, bool revealModifier, bool revealLover, PlayerVoteArea player = null)
        {
            if (CamouflageUnCamouflage.IsCamoed && player == null) return "";
            if (PlayerControl.LocalPlayer.Data.IsDead) return base.NameText(revealTasks, revealRole, revealModifier, revealLover, player);
            if (Revealed) return base.NameText(revealTasks, revealRole, revealModifier, revealLover, player);

            Player.nameText().color = Patches.Colors.Crew;
            if (player != null) player.NameText.color = Patches.Colors.Crew;
            if (player != null && (MeetingHud.Instance.state == MeetingHud.VoteStates.Proceeding ||
                                   MeetingHud.Instance.state == MeetingHud.VoteStates.Results)) return PlayerName;
            Player.nameText().transform.localPosition = new Vector3(
                0f,
                Player.Data.DefaultOutfit.HatId == "hat_NoHat" ? 1.5f : 2.0f,
                -0.5f
            );
            if(Local)
                return PlayerName + "\n" + "Snitch";
            return PlayerName;
        }
        
        public void DestroyArrow(byte targetPlayerId)
        {
            var arrow = SnitchArrows.FirstOrDefault(x => x.Key == targetPlayerId);
            if (arrow.Value != null)
                Object.Destroy(arrow.Value);
            if (arrow.Value.gameObject != null)
                Object.Destroy(arrow.Value.gameObject);
            SnitchArrows.Remove(arrow.Key);
        }
    }
}