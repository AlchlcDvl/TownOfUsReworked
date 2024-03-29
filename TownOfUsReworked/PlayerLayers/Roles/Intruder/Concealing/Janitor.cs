namespace TownOfUsReworked.PlayerLayers.Roles;

public class Janitor : Intruder
{
    public CustomButton CleanButton { get; set; }
    public CustomButton DragButton { get; set; }
    public CustomButton DropButton { get; set; }
    public DeadBody CurrentlyDragging { get; set; }

    public override UColor Color => ClientGameOptions.CustomIntColors ? CustomColorManager.Janitor : CustomColorManager.Intruder;
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
        DragButton = CreateButton(this, new SpriteName("Drag"), AbilityTypes.Dead, KeybindType.Tertiary, (OnClick)Drag, new Cooldown(CustomGameOptions.DragCd), "DRAG BODY",
            (UsableFunc)Usable1);
        DropButton = CreateButton(this, new SpriteName("Drop"), AbilityTypes.Targetless, KeybindType.Tertiary, (OnClick)Drop, "DROP BODY", (UsableFunc)Usable2);
        CleanButton = CreateButton(this, new SpriteName("Clean"), AbilityTypes.Dead, KeybindType.Secondary, (OnClick)Clean, new Cooldown(CustomGameOptions.CleanCd), "CLEAN BODY",
            (UsableFunc)Usable1, (DifferenceFunc)Difference);
    }

    public void Clean()
    {
        Spread(Player, PlayerByBody(CleanButton.TargetBody));
        CallRpc(CustomRPC.Action, ActionsRPC.FadeBody, CleanButton.TargetBody);
        FadeBody(CleanButton.TargetBody);
        CleanButton.StartCooldown();

        if (CustomGameOptions.JaniCooldownsLinked)
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

    public bool Usable1() => !CurrentlyDragging;

    public bool Usable2() => CurrentlyDragging;

    public float Difference() => LastImp && CustomGameOptions.SoloBoost && !Dead ? -CustomGameOptions.UnderdogKillBonus : 0;

    public override void ReadRPC(MessageReader reader)
    {
        CurrentlyDragging = reader.ReadBody();
        DragHandler.Instance.StartDrag(Player, CurrentlyDragging);
    }
}