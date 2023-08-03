namespace TownOfUsReworked.PlayerLayers.Abilities
{
    public class Swapper : Ability
    {
        public PlayerVoteArea Swap1 { get; set; }
        public PlayerVoteArea Swap2 { get; set; }
        public CustomMeeting SwapMenu { get; set; }

        public override Color32 Color => ClientGameOptions.CustomAbColors ? Colors.Swapper : Colors.Ability;
        public override string Name => "Swapper";
        public override LayerEnum Type => LayerEnum.Swapper;
        public override AbilityEnum AbilityType => AbilityEnum.Swapper;
        public override Func<string> Description => () => "- You can swap the votes against 2 players in meetings";

        public Swapper(PlayerControl player) : base(player)
        {
            Swap1 = null;
            Swap2 = null;
            SwapMenu = new(Player, "SwapActive", "SwapDisabled", MeetingTypes.Toggle, CustomGameOptions.SwapAfterVoting, SetActive, IsExempt);
        }

        public override void VoteComplete(MeetingHud __instance)
        {
            base.VoteComplete(__instance);
            SwapMenu.HideButtons();

            if (Swap1 && Swap2)
                CallRpc(CustomRPC.Action, ActionsRPC.SetSwaps, this, Swap1, Swap2);
        }

        private void SetActive(PlayerVoteArea voteArea, MeetingHud __instance)
        {
            if (__instance.state == MeetingHud.VoteStates.Discussion || IsExempt(voteArea))
                return;

            if (Swap1 == null)
                Swap1 = voteArea;
            else if (Swap2 == null)
                Swap2 = voteArea;
            else if (Swap1 == voteArea)
            {
                Swap1 = null;
                SwapMenu.Actives[Swap1.TargetPlayerId] = false;
            }
            else if (Swap2 == voteArea)
            {
                Swap2 = null;
                SwapMenu.Actives[Swap2.TargetPlayerId] = false;
            }
            else
            {
                SwapMenu.Actives[Swap1.TargetPlayerId] = false;
                Swap1 = Swap2;
                Swap2 = voteArea;
            }

            SwapMenu.Actives[voteArea.TargetPlayerId] = !SwapMenu.Actives[voteArea.TargetPlayerId];
        }

        public override void OnMeetingStart(MeetingHud __instance)
        {
            base.OnMeetingStart(__instance);
            SwapMenu.GenButtons(__instance);
        }

        private bool IsExempt(PlayerVoteArea voteArea)
        {
            var player = PlayerByVoteArea(voteArea);
            return player.Data.IsDead || player.Data.Disconnected || (Local && Player == player && !CustomGameOptions.SwapSelf) || IsDead;
        }

        public override void ConfirmVotePrefix(MeetingHud __instance)
        {
            base.ConfirmVotePrefix(__instance);
            SwapMenu.Voted();

            if (Swap1 && Swap2)
                CallRpc(CustomRPC.Action, ActionsRPC.SetSwaps, this, Swap1, Swap2);
        }

        public override void UpdateMeeting(MeetingHud __instance)
        {
            base.UpdateMeeting(__instance);
            SwapMenu.Update();
        }
    }
}