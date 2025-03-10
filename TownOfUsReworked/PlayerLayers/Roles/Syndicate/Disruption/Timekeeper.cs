namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Timekeeper)]
public sealed class Timekeeper : Syndicate, ITimeLord
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number TimeCd = 25;

    [NumberOption(5f, 30f, 1f, Format.Time)]
    public static Number TimeDur = 10;

    [ToggleOption]
    public static bool TimeFreezeImmunity = true;

    [ToggleOption]
    public static bool TimeRewindImmunity = true;

    public static bool TkExists { get; private set; }

    public CustomButton TimeButton { get; private set; }

    public override UColor Color => ClientOptions.CustomSynColors ? CustomColorManager.Timekeeper : FactionColor;
    public override LayerEnum Type => LayerEnum.Timekeeper;
    public override Func<string> StartText => () => "Bend Time To Your Will";
    public override Func<string> Description => () => $"- You can {(HoldsDrive ? "rewind" : "freeze")} time, making people {(HoldsDrive ? "go backwards" : "unable to move")}\n" +
        CommonAbilities;

    protected override void Init()
    {
        base.Init();
        Alignment = Alignment.Disruption;
        TimeButton ??= new(this, new SpriteName("Time"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClickTargetless)TimeControl, new Cooldown(TimeCd), (LabelFunc)Label,
            (EffectEndVoid)UnControl, new Duration(TimeDur), (EffectVoid)Control, (EffectStartVoid)ControlStart);
        TkExists = true;
    }

    protected override void Deinit()
    {
        base.Deinit();
        TkExists = false;
    }

    private void ControlStart() => Flash(Color, TimeDur);

    private void Control()
    {
        if (HoldsDrive)
            AllPlayers().ForEach(x => x.GetRole().Rewinding = true);
    }

    public static void UnControl() => AllPlayers().ForEach(x => x.GetRole().Rewinding = false);

    private void TimeControl()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, TimeButton);
        TimeButton.Begin();
    }

    private string Label() => HoldsDrive ? "REWIND" : "FREEZE";
}