namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(Layer.Dictator)]
public sealed class Dictator : Sovereign
{
    [ToggleOption]
    private static bool RoundOneNoDictReveal = false;

    [ToggleOption]
    public static bool DictatorButton = true;

    [NumberOption(0, 10, 1, zeroIsInf: true)]
    private static Number MaxTribunals = 2;

    private bool RoundOne;
    public bool Tribunal;
    public PlayerControl ToBeEjected;
    private CustomButton RevealButton;
    public CustomMeeting DictMenu;

    protected override UColor MainColor => CustomColorManager.Dictator;
    public override Layer Type => Layer.Dictator;
    public override string StartText => "You Have The Final Say";
    public override string Description => "- You can reveal yourself to the crew to eject up to 3 players for one meeting\n- When revealed, you cannot be protected";

    public override void Init()
    {
        RevealButton ??= new(this, "REVEAL", new SpriteName("DictReveal"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClickTargetless)Reveal, (UsableFunc)Usable, MaxTribunals);
        DictMenu = new(Player, "DictActive", "DictDisabled", SetActive, IsExempt, new(-0.4f, 0.03f, -1.3f));
    }

    public override void Reset(bool meeting, bool start) => RoundOne = start && RoundOneNoDictReveal;

    private void Reveal()
    {
        if (!Revealed && !GetLayers<Dictator>().Any(x => !x.TrulyDead && x.Revealed))
        {
            CallRpc(ActionsRpc.PublicReveal, Player);
            PublicReveal(Player);
        }
        else
            OnReveal();
    }

    public override void OnReveal()
    {
        if (Local)
            PerformRpcAction(DictActionsRpc.Tribunal);

        Tribunal = true;
    }

    public override void VoteComplete(MeetingHud __instance)
    {
        DictMenu.HideButtons();

        if (ToBeEjected && !Dead)
            PerformRpcAction(DictActionsRpc.SelectToEject, ToBeEjected);
    }

    private void SetActive(PlayerVoteArea voteArea, MeetingHud __instance)
    {
        if (__instance.state == MeetingHud.VoteStates.Discussion || !Revealed || Dead)
            return;

        var id = voteArea.TargetPlayerId;
        var player = PlayerById(id);
        DictMenu.Actives[id] = !DictMenu.Actives[id];
        var prev = ToBeEjected;

        if (!prev)
            ToBeEjected = player;
        else if (prev == player)
            ToBeEjected = null;
        else if (prev)
            DictMenu.Actives[prev.PlayerId] = false;
    }

    public override void LocalOnMeetingStart(MeetingHud __instance) => DictMenu.GenButtons(__instance, Tribunal && !Dead);

    public override void OnMeetingStart(MeetingHud __instance)
    {
        ToBeEjected = null;
        Tribunal = false;
    }

    private bool IsExempt(PlayerVoteArea voteArea)
    {
        var player = PlayerByVoteArea(voteArea);
        return player.HasDied() || player.AmOwner || Dead || !Revealed;
    }

    public override void UpdateMeeting(MeetingHud __instance) => DictMenu.Update();

    private bool Usable() => !RoundOne && !Tribunal;

    public override void ReadRPC(RpcReader reader)
    {
        var dictAction = reader.Read<DictActionsRpc>();

        switch (dictAction)
        {
            case DictActionsRpc.Tribunal:
            {
                Flash(Color);
                Tribunal = true;
                break;
            }
            case DictActionsRpc.SelectToEject:
            {
                ToBeEjected = reader.ReadPlayer();
                break;
            }
            default:
            {
                Failure($"Received unknown RPC - {dictAction}");
                break;
            }
        }
    }

    public override void UpdateSelfName(ref string name, ref UColor color, ref bool revealed, ref bool removeFromConsig)
    {
        if (!Revealed)
            return;

        revealed = true;
        name += $"\n{Name}";
        color = Color;
        removeFromConsig = true;
    }
}