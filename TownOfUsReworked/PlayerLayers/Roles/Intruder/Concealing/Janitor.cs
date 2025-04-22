namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Janitor)]
public sealed class Janitor : Intruder
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number CleanCd = 25;

    [ToggleOption]
    public static bool JaniCooldownsLinked = true;

    [ToggleOption]
    public static bool SoloBoost = false;

    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number DragCd = 25;

    [NumberOption(0.25f, 3f, 0.25f, Format.Multiplier)]
    public static Number DragModifier = 0.5f;

    [StringOption<JanitorOptions>]
    public static JanitorOptions JanitorVentOptions = JanitorOptions.Never;

    private CustomButton CleanButton { get; set; }
    private CustomButton DragButton { get; set; }
    private CustomButton DropButton { get; set; }
    public DeadBodyHandler CurrentlyDragging { get; set; }

    protected override UColor MainColor => CustomColorManager.Janitor;
    public override LayerEnum Type => LayerEnum.Janitor;
    public override Func<string> StartText { get; } = () => "Sanitise The Ship, By Any Means Necessary";
    public override Func<string> Description => () => "- You can clean up dead bodies, making them disappear from sight\n- You can drag bodies away to prevent them from getting reported\n" +
        CommonAbilities;
    public override bool CanVent => base.CanVent && ((int)JanitorVentOptions is 3 || (CurrentlyDragging && (int)JanitorVentOptions is 1) || (!CurrentlyDragging && (int)JanitorVentOptions is 2));

    protected override void Init()
    {
        base.Init();
        Alignment = Alignment.Concealing;
        CurrentlyDragging = null;
        DragButton ??= new(this, new SpriteName("Drag"), AbilityTypes.Body, KeybindType.Tertiary, (OnClickBody)Drag, new Cooldown(DragCd), "DRAG BODY", (UsableFunc)Usable1);
        DropButton ??= new(this, new SpriteName("Drop"), AbilityTypes.Targetless, KeybindType.Tertiary, (OnClickTargetless)Drop, "DROP BODY", (UsableFunc)Usable2);
        CleanButton ??= new(this, new SpriteName("Clean"), AbilityTypes.Body, KeybindType.Secondary, (OnClickBody)Clean, new Cooldown(CleanCd), "CLEAN BODY", (DifferenceFunc)Difference,
            (UsableFunc)Usable1);
    }

    public override void Reset(bool meeting, bool start) => CurrentlyDragging = null;

    private void Clean(DeadBody target)
    {
        Spread(Player, PlayerByBody(target));
        CallRpc(CustomRPC.Action, ActionsRPC.FadeBody, target);
        FadeBody(target);
        CleanButton.StartCooldown();

        if (JaniCooldownsLinked)
            KillButton.StartCooldown();
    }

    private void Drag(DeadBody target)
    {
        target.GetComponent<DeadBodyHandler>().StartDrag(Player);
        Spread(Player, PlayerByBody(target));
        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, target);
    }

    public void Drop()
    {
        if (!CurrentlyDragging)
            return;

        CallRpc(CustomRPC.Action, ActionsRPC.Drop, CurrentlyDragging.Body);
        CurrentlyDragging.StopDrag();
        DragButton.StartCooldown();
    }

    public override void OnDeath(DeathReasonEnum reason, PlayerControl killer)
    {
        if (Local)
            Drop();
    }

    private bool Usable1() => !DeadBodyHandler.Dragging.Contains(PlayerId);

    private bool Usable2() => DeadBodyHandler.Dragging.Contains(PlayerId);

    private float Difference() => Last(Faction) && SoloBoost && !Dead ? -Underdog.UnderdogCdBonus : 0;

    public override void ReadRPC(NetData reader) => reader.ReadBody().GetComponent<DeadBodyHandler>().StartDrag(Player);

    protected override void Kill(PlayerControl target)
    {
        base.Kill(target);

        if (JaniCooldownsLinked)
            CleanButton.StartCooldown(CooldownType.Custom, KillButton.CooldownTime);
    }
}