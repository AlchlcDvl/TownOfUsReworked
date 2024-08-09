namespace TownOfUsReworked.PlayerLayers.Abilities;

public class Swapper : Ability
{
    public PlayerVoteArea Swap1 { get; set; }
    public PlayerVoteArea Swap2 { get; set; }
    public CustomMeeting SwapMenu { get; set; }

    public override UColor Color => ClientOptions.CustomAbColors ? CustomColorManager.Swapper : CustomColorManager.Ability;
    public override string Name => "Swapper";
    public override LayerEnum Type => LayerEnum.Swapper;
    public override Func<string> Description => () => "- You can swap the votes against 2 players in meetings";

    public override void Init()
    {
        Swap1 = null;
        Swap2 = null;
        SwapMenu = new(Player, "SwapActive", "SwapDisabled", CustomGameOptions.SwapAfterVoting, SetActive, IsExempt, position: null);
    }

    public override void VoteComplete(MeetingHud __instance)
    {
        base.VoteComplete(__instance);
        SwapMenu.HideButtons();

        if (Swap1 && Swap2)
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, Swap1, Swap2);
    }

    public override void ReadRPC(MessageReader reader)
    {
        Swap1 = reader.ReadVoteArea();
        Swap2 = reader.ReadVoteArea();
    }

    private void SetActive(PlayerVoteArea voteArea, MeetingHud __instance)
    {
        if (__instance.state == MeetingHud.VoteStates.Discussion || IsExempt(voteArea))
            return;

        if (!Swap1)
        {
            Swap1 = voteArea;
            SwapMenu.Actives[voteArea.TargetPlayerId] = true;
        }
        else if (!Swap2)
        {
            Swap2 = voteArea;
            SwapMenu.Actives[voteArea.TargetPlayerId] = true;
        }
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
            SwapMenu.Actives[voteArea.TargetPlayerId] = !SwapMenu.Actives[voteArea.TargetPlayerId];
        }
    }

    public override void OnMeetingStart(MeetingHud __instance)
    {
        base.OnMeetingStart(__instance);
        SwapMenu.GenButtons(__instance);
    }

    private bool IsExempt(PlayerVoteArea voteArea)
    {
        var player = PlayerByVoteArea(voteArea);
        return player.HasDied() || (Local && Player == player && !CustomGameOptions.SwapSelf) || Dead;
    }

    public override void ConfirmVotePrefix(MeetingHud __instance)
    {
        base.ConfirmVotePrefix(__instance);
        SwapMenu.Voted();

        if (Swap1 && Swap2)
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, Swap1, Swap2);
    }

    public override void UpdateMeeting(MeetingHud __instance)
    {
        base.UpdateMeeting(__instance);
        SwapMenu.Update(__instance);
    }
}