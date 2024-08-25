namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Janitor : Intruder
{
    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static float CleanCd { get; set; } = 25f;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool JaniCooldownsLinked { get; set; } = true;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool SoloBoost { get; set; } = false;

    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static float DragCd { get; set; } = 25f;

    [NumberOption(MultiMenu.LayerSubOptions, 0.25f, 3f, 0.25f, Format.Multiplier)]
    public static float DragModifier { get; set; } = 0.5f;

    [StringOption(MultiMenu.LayerSubOptions)]
    public static JanitorOptions JanitorVentOptions { get; set; } = JanitorOptions.Never;

    public CustomButton CleanButton { get; set; }
    public CustomButton DragButton { get; set; }
    public CustomButton DropButton { get; set; }
    public DeadBody CurrentlyDragging { get; set; }

    public override UColor Color => ClientOptions.CustomIntColors ? CustomColorManager.Janitor : CustomColorManager.Intruder;
    public override string Name => "Janitor";
    public override LayerEnum Type => LayerEnum.Janitor;
    public override Func<string> StartText => () => "Sanitise The Ship, By Any Means Neccessary";
    public override Func<string> Description => () => "- You can clean up dead bodies, making them disappear from sight\n- You can drag bodies away to prevent them from getting reported\n" +
        CommonAbilities;

    public override void Init()
    {
        BaseStart();
        Alignment = Alignment.IntruderConceal;
        CurrentlyDragging = null;
        DragButton = CreateButton(this, new SpriteName("Drag"), AbilityTypes.Dead, KeybindType.Tertiary, (OnClick)Drag, new Cooldown(DragCd), "DRAG BODY", (UsableFunc)Usable1);
        DropButton = CreateButton(this, new SpriteName("Drop"), AbilityTypes.Targetless, KeybindType.Tertiary, (OnClick)Drop, "DROP BODY", (UsableFunc)Usable2);
        CleanButton = CreateButton(this, new SpriteName("Clean"), AbilityTypes.Dead, KeybindType.Secondary, (OnClick)Clean, new Cooldown(CleanCd), "CLEAN BODY", (DifferenceFunc)Difference,
            (UsableFunc)Usable1);
    }

    public void Clean()
    {
        Spread(Player, PlayerByBody(CleanButton.TargetBody));
        CallRpc(CustomRPC.Action, ActionsRPC.FadeBody, CleanButton.TargetBody);
        FadeBody(CleanButton.TargetBody);
        CleanButton.StartCooldown();

        if (JaniCooldownsLinked)
            KillButton.StartCooldown();
    }

    public void Drag()
    {
        CurrentlyDragging = DragButton.TargetBody;
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

    public float Difference() => LastImp && SoloBoost && !Dead ? -Underdog.UnderdogKillBonus : 0;

    public override void ReadRPC(MessageReader reader)
    {
        CurrentlyDragging = reader.ReadBody();
        DragHandler.Instance.StartDrag(Player, CurrentlyDragging);
    }
}