namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Timekeeper)]
public sealed class Timekeeper : Syndicate
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
    public static Faction TkFaction { get; private set; }

    public CustomButton TimeButton { get; private set; }

    protected override UColor MainColor => CustomColorManager.Timekeeper;
    public override LayerEnum Type => LayerEnum.Timekeeper;
    public override Func<string> StartText { get; } = () => "Bend Time To Your Will";
    public override Func<string> Description => () => $"- You can {(HoldsDrive ? "rewind" : "freeze")} time, making people {(HoldsDrive ? "go backwards" : "unable to move")}\n" +
        CommonAbilities;

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.Disruption;
        TimeButton ??= new(this, new SpriteName("Time"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClickTargetless)TimeControl, new Cooldown(TimeCd), (LabelFunc)Label,
            (EffectEndVoid)UnControl, new Duration(TimeDur), (EffectStartVoid)ControlStart, (UsableFunc)Usable);
        TkExists = true;
    }

    protected override void Deinit() => TkExists = false;

    private void ControlStart()
    {
        Flash(Color, TimeDur);

        if (!HoldsDrive)
            return;

        AllPlayers().Do(x => LayerHandler.Handlers[x.PlayerId].Rewinding = true);
        TkFaction = Handler.CurrentFaction;
    }

    public static void UnControl()
    {
        AllPlayers().Do(x => LayerHandler.Handlers[x.PlayerId].Rewinding = false);
        TkFaction = Faction.None;
    }

    private void TimeControl()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, TimeButton);
        TimeButton.Begin();
    }

    private string Label() => HoldsDrive ? "REWIND" : "FREEZE";

    private bool Usable() => (!HoldsDrive || TkFaction == Faction.None) && !Handler.Rewinding;
}