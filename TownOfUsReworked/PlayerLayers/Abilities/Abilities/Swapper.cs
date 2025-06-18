namespace TownOfUsReworked.PlayerLayers.Abilities;

[LayerHeaderOption(Layer.Swapper)]
public sealed class Swapper : Ability
{
    [ToggleOption]
    public static bool SwapperButton = true;

    [ToggleOption]
    private static bool SwapSelf = true;

    public PlayerVoteArea Swap1;
    public PlayerVoteArea Swap2;
    public CustomMeeting SwapMenu;

    protected override UColor MainColor => CustomColorManager.Swapper;
    public override Layer Type => Layer.Swapper;
    public override string Description => "- You can swap the votes against 2 players in meetings";

    public override void Init()
    {
        Swap1 = null;
        Swap2 = null;
        SwapMenu = new(Player, "SwapActive", "SwapDisabled", SetActive, IsExempt, position: null);
    }

    public override void VoteComplete(MeetingHud __instance)
    {
        SwapMenu.HideButtons();

        if (Swap1 && Swap2 && !Dead)
            PerformRpcAction(Swap1, Swap2);
    }

    public override void ReadRPC(RpcReader reader)
    {
        Swap1 = reader.ReadVoteArea();
        Swap2 = reader.ReadVoteArea();
    }

    public override void OnMeetingStart(MeetingHud __instance) => SwapMenu.GenButtons(__instance);

    public override void UpdateMeeting(MeetingHud __instance) => SwapMenu.Update();

    private bool IsExempt(PlayerVoteArea voteArea)
    {
        var player = PlayerByVoteArea(voteArea);
        return player.HasDied() || (Local && Player == player && !SwapSelf) || Dead;
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
            SwapMenu.Actives[Swap1.TargetPlayerId] = false;
            Swap1 = null;
        }
        else if (Swap2 == voteArea)
        {
            SwapMenu.Actives[Swap2.TargetPlayerId] = false;
            Swap2 = null;
        }
        else
        {
            SwapMenu.Actives[Swap1.TargetPlayerId] = false;
            Swap1 = Swap2;
            Swap2 = voteArea;
            SwapMenu.Actives[voteArea.TargetPlayerId] = !SwapMenu.Actives[voteArea.TargetPlayerId];
        }
    }
}