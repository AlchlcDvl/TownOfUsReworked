namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Janitor : Intruder, IDragger
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

    public CustomButton CleanButton { get; set; }
    public CustomButton DragButton { get; set; }
    public CustomButton DropButton { get; set; }
    public DeadBody CurrentlyDragging { get; set; }

    public override UColor Color => ClientOptions.CustomIntColors ? CustomColorManager.Janitor : FactionColor;
    public override LayerEnum Type => LayerEnum.Janitor;
    public override Func<string> StartText => () => "Sanitise The Ship, By Any Means Neccessary";
    public override Func<string> Description => () => "- You can clean up dead bodies, making them disappear from sight\n- You can drag bodies away to prevent them from getting reported\n" +
        CommonAbilities;

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.Concealing;
        CurrentlyDragging = null;
        DragButton ??= new(this, new SpriteName("Drag"), AbilityTypes.Body, KeybindType.Tertiary, (OnClickBody)Drag, new Cooldown(DragCd), "DRAG BODY", (UsableFunc)Usable1);
        DropButton ??= new(this, new SpriteName("Drop"), AbilityTypes.Targetless, KeybindType.Tertiary, (OnClickTargetless)Drop, "DROP BODY", (UsableFunc)Usable2);
        CleanButton ??= new(this, new SpriteName("Clean"), AbilityTypes.Body, KeybindType.Secondary, (OnClickBody)Clean, new Cooldown(CleanCd), "CLEAN BODY", (DifferenceFunc)Difference,
            (UsableFunc)Usable1);
    }

    public void Clean(DeadBody target)
    {
        Spread(Player, PlayerByBody(target));
        CallRpc(CustomRPC.Action, ActionsRPC.FadeBody, target);
        FadeBody(target);
        CleanButton.StartCooldown();

        if (JaniCooldownsLinked)
            KillButton.StartCooldown();
    }

    public void Drag(DeadBody target)
    {
        CurrentlyDragging = target;
        Spread(Player, PlayerByBody(CurrentlyDragging));
        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, CurrentlyDragging);
        DragHandler.Instance.StartDrag(Player, CurrentlyDragging);
    }

    public void Drop()
    {
        if (!CurrentlyDragging)
            return;

        CallRpc(CustomRPC.Action, ActionsRPC.Drop, Player);
        DragHandler.Instance.StopDrag(Player);
        CurrentlyDragging = null;
        DragButton.StartCooldown();
    }

    public bool Usable1() => !DragHandler.Instance.Dragging.ContainsKey(PlayerId);

    public bool Usable2() => DragHandler.Instance.Dragging.ContainsKey(PlayerId);

    public float Difference() => Last(Faction) && SoloBoost && !Dead ? -Underdog.UnderdogCdBonus : 0;

    public override void ReadRPC(MessageReader reader)
    {
        CurrentlyDragging = reader.ReadBody();
        DragHandler.Instance.StartDrag(Player, CurrentlyDragging);
    }

    public override void Kill(PlayerControl target)
    {
        base.Kill(target);

        if (JaniCooldownsLinked)
            CleanButton.StartCooldown(CooldownType.Custom, KillButton.CooldownTime);
    }
}