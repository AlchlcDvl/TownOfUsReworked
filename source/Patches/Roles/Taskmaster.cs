using TownOfUs.Extensions;
using UnityEngine;
using System.Linq;

namespace TownOfUs.Roles
{
    public class Taskmaster : Role
    {

        public bool Revealed => TasksLeft <= CustomGameOptions.TMTasksRemaining;
        public bool TasksDone => TasksLeft <= 0;
        public int TasksToBeDone;
        public bool WinTasksDone;
        public System.Collections.Generic.List<ArrowBehaviour> ImpArrows = new System.Collections.Generic.List<ArrowBehaviour>();
        public System.Collections.Generic.Dictionary<byte, ArrowBehaviour> TMArrows = new System.Collections.Generic.Dictionary<byte, ArrowBehaviour>();

        public Taskmaster(PlayerControl player) : base(player)
        {
            Name = "Taskmaster";
            ImpostorText = () => "Finish Your Tasks";
            TaskText = () => "Do something that no one has ever done before! Finish your tasks";
            if (CustomGameOptions.CustomNeutColors) Color = Patches.Colors.Taskmaster;
            else Color = Patches.Colors.Neutral;
            RoleType = RoleEnum.Taskmaster;
            Faction = Faction.Neutral;
            FactionName = "Neutral";
            FactionColor = Patches.Colors.Neutral;
            Alignment = Alignment.NeutralEvil;
            AlignmentName = "Neutral (Evil)";
            AddToRoleHistory(RoleType);
        }

        public bool TaskmasterWins { get; set; }

        public void Wins()
        {
            TaskmasterWins = true;
        }

        public void Loses()
        {
            LostByRPC = true;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var tmTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            tmTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = tmTeam;
        }

        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (!WinTasksDone || !Player.Data.IsDead && !Player.Data.Disconnected) return true;
            Utils.EndGame();
            return false;
        }

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
            Player.nameText().transform.localPosition = new Vector3(0f, Player.Data.DefaultOutfit.HatId == "hat_NoHat" ? 1.5f : 2.0f, -0.5f);
            if(Local)
                return PlayerName + "\n" + "Taskmaster";
            return PlayerName;
        }

        public void DestroyArrow(byte targetPlayerId)
        {
            var arrow = TMArrows.FirstOrDefault(x => x.Key == targetPlayerId);
            if (arrow.Value != null)
                Object.Destroy(arrow.Value);
            if (arrow.Value.gameObject != null)
                Object.Destroy(arrow.Value.gameObject);
            TMArrows.Remove(arrow.Key);
        }
    }
}