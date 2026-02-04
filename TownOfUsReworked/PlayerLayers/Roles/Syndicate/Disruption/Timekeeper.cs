namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(Layer.Timekeeper)]
public sealed class Timekeeper : Disruption, ISpeedModifier
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number TimeCd = 25;

    [NumberOption(5f, 30f, 1f, Format.Time)]
    public static Number TimeDur = 10;

    [ToggleOption]
    public static bool TimeFreezeImmunity = true;

    [ToggleOption]
    public static bool TimeRewindImmunity = true;

    public static bool TkExists;
    public static Faction TkFaction;

    public CustomButton TimeButton;

    protected override UColor MainColor => CustomColorManager.Timekeeper;
    public override Layer Type => Layer.Timekeeper;
    public override string StartText => "Bend Time To Your Will";
    public override string Description => $"- You can {(HoldsDrive ? "rewind" : "freeze")} time, making people {(HoldsDrive ? "go backwards" : "unable to move")}\n" +
        CommonAbilities;

    public override void Init()
    {
        base.Init();
        TimeButton ??= new(this, new SpriteName("Time"), ReworkedAbilityTypes.Targetless, KeybindType.Secondary, (OnClickTargetless)TimeControl, new Cooldown(TimeCd), (LabelFunc)Label,
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

    private static void UnControl()
    {
        AllPlayers().Do(x => LayerHandler.Handlers[x.PlayerId].Rewinding = false);
        TkFaction = Faction.None;
    }

    private void TimeControl() => TimeButton.TriggerRpcAndBegin();

    private string Label() => HoldsDrive ? "REWIND" : "FREEZE";

    private bool Usable() => (!HoldsDrive || TkFaction == Faction.None) && !Handler.Rewinding;

    public void ModifySpeed(PlayerControl player, ref float result)
    {
        if (!TimeButton.EffectActive)
            return;

        // (tk. ||

        if ((Handler.CurrentFaction.IsFactionedEvil() && !Player.Is(Handler.CurrentFaction)) || (!HoldsDrive && !TimeFreezeImmunity) || (HoldsDrive && !TimeRewindImmunity))
            result = 0f;
    }
}