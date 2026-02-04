namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(Layer.Janitor)]
public sealed class Janitor : Concealing
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number CleanCd = 25;

    [ToggleOption]
    private static bool JaniCooldownsLinked = true;

    [ToggleOption]
    private static bool SoloBoost = false;

    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number DragCd = 25;

    [NumberOption(0.25f, 3f, 0.25f, Format.Multiplier)]
    public static Number DragModifier = 0.5f;

    [StringOption<JanitorOptions>]
    private static JanitorOptions JanitorVentOptions = JanitorOptions.Never;

    private CustomButton CleanButton;
    private CustomButton DragButton;
    private CustomButton DropButton;
    public DeadBodyHandler CurrentlyDragging;

    protected override UColor MainColor => CustomColorManager.Janitor;
    public override Layer Type => Layer.Janitor;
    public override string StartText => "Sanitise The Ship, By Any Means Necessary";
    public override string Description => "- You can clean up dead bodies, making them disappear from sight\n- You can drag bodies away to prevent them from getting reported\n" +
        CommonAbilities;
    public override bool CanVent => base.CanVent && ((int)JanitorVentOptions is 3 || (CurrentlyDragging && (int)JanitorVentOptions is 1) || (!CurrentlyDragging && (int)JanitorVentOptions is 2));

    public override void Init()
    {
        base.Init();
        CurrentlyDragging = null;
        DragButton ??= new(this, new SpriteName("Drag"), ReworkedAbilityTypes.Body, KeybindType.Tertiary, (OnClickBody)Drag, new Cooldown(DragCd), "DRAG BODY", (UsableFunc)Usable1);
        DropButton ??= new(this, new SpriteName("Drop"), ReworkedAbilityTypes.Targetless, KeybindType.Tertiary, (OnClickTargetless)Drop, "DROP BODY", (UsableFunc)Usable2);
        CleanButton ??= new(this, new SpriteName("Clean"), ReworkedAbilityTypes.Body, KeybindType.Secondary, (OnClickBody)Clean, new Cooldown(CleanCd), "CLEAN BODY", (DifferenceFunc)Difference,
            (UsableFunc)Usable1);
    }

    public override void Reset(bool meeting, bool start) => CurrentlyDragging = null;

    private void Clean(DeadBody target)
    {
        Spread(Player, PlayerByBody(target));
        CallRpc(ActionsRpc.FadeBody, target);
        FadeBody(target);
        CleanButton.StartCooldown();

        if (JaniCooldownsLinked)
            KillButton.StartCooldown();
    }

    private void Drag(DeadBody target)
    {
        target.GetComponent<DeadBodyHandler>().StartDrag(Player);
        Spread(Player, PlayerByBody(target));
        PerformRpcAction(target);
    }

    public void Drop()
    {
        if (!CurrentlyDragging)
            return;

        CallRpc(ActionsRpc.Drop, CurrentlyDragging.Body);
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

    private float Difference() => Last(Handler.CurrentFaction) && SoloBoost && !Dead ? -Underdog.UnderdogCdBonus : 0;

    public override void ReadRPC(RpcReader reader) => reader.ReadBody().GetComponent<DeadBodyHandler>().StartDrag(Player);

    protected override void Kill(PlayerControl target)
    {
        base.Kill(target);

        if (JaniCooldownsLinked)
            CleanButton.StartCooldown();
    }
}