namespace TownOfUsReworked.PlayerLayers.Roles;

public class Janitor : Intruder
{
    public CustomButton CleanButton { get; set; }
    public CustomButton DragButton { get; set; }
    public CustomButton DropButton { get; set; }
    public DeadBody CurrentlyDragging { get; set; }

    public override Color Color => ClientGameOptions.CustomIntColors ? Colors.Janitor : Colors.Intruder;
    public override string Name => "Janitor";
    public override LayerEnum Type => LayerEnum.Janitor;
    public override Func<string> StartText => () => "Sanitise The Ship, By Any Means Neccessary";
    public override Func<string> Description => () => "- You can clean up dead bodies, making them disappear from sight\n- You can drag bodies away to prevent them from getting " +
        $"reported\n{CommonAbilities}";

    public Janitor(PlayerControl player) : base(player)
    {
        Alignment = Alignment.IntruderConceal;
        CurrentlyDragging = null;
        DragButton = new(this, "Drag", AbilityTypes.Dead, "Tertiary", Drag, CustomGameOptions.DragCd);
        DropButton = new(this, "Drop", AbilityTypes.Targetless, "Tertiary", Drop);
        CleanButton = new(this, "Clean", AbilityTypes.Dead, "Secondary", Clean, CustomGameOptions.CleanCd);
    }

    public void Clean()
    {
        Spread(Player, PlayerByBody(CleanButton.TargetBody));
        CallRpc(CustomRPC.Action, ActionsRPC.FadeBody, this, CleanButton.TargetBody);
        Coroutines.Start(FadeBody(CleanButton.TargetBody));
        CleanButton.StartCooldown();

        if (CustomGameOptions.JaniCooldownsLinked)
            KillButton.StartCooldown();
    }

    public void Drag()
    {
        CurrentlyDragging = DragButton.TargetBody;
        Spread(Player, PlayerByBody(CurrentlyDragging));
        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction2, this, CurrentlyDragging);
        var drag = CurrentlyDragging.gameObject.AddComponent<DragBehaviour>();
        drag.Source = Player;
        drag.Dragged = CurrentlyDragging;
    }

    public void Drop()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.Drop, CurrentlyDragging);
        CurrentlyDragging.gameObject.GetComponent<DragBehaviour>().Destroy();
        CurrentlyDragging = null;
        DragButton.StartCooldown();
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        CleanButton.Update2("CLEAN BODY", CurrentlyDragging == null, difference: LastImp && CustomGameOptions.SoloBoost && !IsDead ? -CustomGameOptions.UnderdogKillBonus : 0);
        DragButton.Update2("DRAG BODY", CurrentlyDragging == null);
        DropButton.Update2("DROP BODY", !DragButton.Usable);
    }

    public override void ReadRPC(MessageReader reader)
    {
        CurrentlyDragging = reader.ReadBody();
        var drag = CurrentlyDragging.gameObject.AddComponent<DragBehaviour>();
        drag.Source = Player;
        drag.Dragged = CurrentlyDragging;
    }
}