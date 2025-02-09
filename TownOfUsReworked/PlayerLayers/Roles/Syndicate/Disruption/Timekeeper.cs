namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Timekeeper : Syndicate
{
    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number TimeCd { get; set; } = new(25);

    [NumberOption(MultiMenu.LayerSubOptions, 5f, 30f, 1f, Format.Time)]
    public static Number TimeDur { get; set; } = new(10);

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool TimeFreezeImmunity { get; set; } = true;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool TimeRewindImmunity { get; set; } = true;

    public CustomButton TimeButton { get; set; }

    public override UColor Color => ClientOptions.CustomSynColors ? CustomColorManager.Timekeeper : CustomColorManager.Syndicate;
    public override string Name => "Timekeeper";
    public override LayerEnum Type => LayerEnum.Timekeeper;
    public override Func<string> StartText => () => "Bend Time To Your Will";
    public override Func<string> Description => () => $"- You can {(HoldsDrive ? "rewind" : "freeze")} time, making people {(HoldsDrive ? "go backwards" : "unable to move")}\n" +
        CommonAbilities;

    public override void Init()
    {
        BaseStart();
        Alignment = Alignment.SyndicateDisrup;
        TimeButton ??= CreateButton(this, new SpriteName("Time"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClick)TimeControl, new Cooldown(TimeCd), (LabelFunc)Label,
            new Duration(TimeDur), (EffectVoid)Control, (EffectStartVoid)ControlStart, (EffectEndVoid)UnControl);
        Data.Role.IntroSound = GetAudio("TimekeeperIntro");
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