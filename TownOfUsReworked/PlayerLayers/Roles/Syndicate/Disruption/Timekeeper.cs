namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Timekeeper : Syndicate
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number TimeCd = 25;

    [NumberOption(5f, 30f, 1f, Format.Time)]
    public static Number TimeDur = 10;

    [ToggleOption]
    public static bool TimeFreezeImmunity = true;

    [ToggleOption]
    public static bool TimeRewindImmunity = true;

    public static bool TKExists { get; set; }

    public CustomButton TimeButton { get; set; }

    public override UColor Color => ClientOptions.CustomSynColors ? CustomColorManager.Timekeeper : FactionColor;
    public override LayerEnum Type => LayerEnum.Timekeeper;
    public override Func<string> StartText => () => "Bend Time To Your Will";
    public override Func<string> Description => () => $"- You can {(HoldsDrive ? "rewind" : "freeze")} time, making people {(HoldsDrive ? "go backwards" : "unable to move")}\n" +
        CommonAbilities;

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.Disruption;
        TimeButton ??= new(this, new SpriteName("Time"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClickTargetless)TimeControl, new Cooldown(TimeCd), (LabelFunc)Label,
            (EffectEndVoid)UnControl, new Duration(TimeDur), (EffectVoid)Control, (EffectStartVoid)ControlStart);
        TKExists = true;
    }

    public override void Deinit()
    {
        base.Deinit();
        TKExists = false;
    }

    public void ControlStart() => Flash(Color, TimeDur);

    public void Control()
    {
        if (HoldsDrive)
            AllPlayers().ForEach(x => x.GetRole().Rewinding = true);
    }

    public void UnControl() => AllPlayers().ForEach(x => x.GetRole().Rewinding = false);

    public void TimeControl()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, TimeButton);
        TimeButton.Begin();
    }

    public string Label() => HoldsDrive ? "REWIND" : "FREEZE";
}