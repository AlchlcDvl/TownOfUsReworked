namespace TownOfUsReworked.PlayerLayers.Roles;

public class Janitor : Intruder
{
    public CustomButton CleanButton { get; set; }
    public CustomButton DragButton { get; set; }
    public CustomButton DropButton { get; set; }
    public DateTime LastDragged { get; set; }
    public DeadBody CurrentlyDragging { get; set; }
    public DateTime LastCleaned { get; set; }

    public override Color Color => ClientGameOptions.CustomIntColors ? Colors.Janitor : Colors.Intruder;
    public override string Name => "Janitor";
    public override LayerEnum Type => LayerEnum.Janitor;
    public override Func<string> StartText => () => "You Know Their Secrets";
    public override Func<string> Description => () => "- You can clean up dead bodies, making them disappear from sight\n- You can drag bodies away to prevent them from getting " +
        $"reported\n{CommonAbilities}";
    public override InspectorResults InspectorResults => InspectorResults.DealsWithDead;
    public float CleanTimer => ButtonUtils.Timer(Player, LastCleaned, CustomGameOptions.CleanCd, LastImp && CustomGameOptions.SoloBoost ? -CustomGameOptions.UnderdogKillBonus :
        0);
    public float DragTimer => ButtonUtils.Timer(Player, LastDragged, CustomGameOptions.DragCd);

    public Janitor(PlayerControl player) : base(player)
    {
        Alignment = Alignment.IntruderConceal;
        CurrentlyDragging = null;
        CleanButton = new(this, "Clean", AbilityTypes.Dead, "Secondary", Clean);
        DragButton = new(this, "Drag", AbilityTypes.Dead, "Tertiary", Drag);
        DropButton = new(this, "Drop", AbilityTypes.Effect, "Tertiary", Drop);
    }

    public void Clean()
    {
        if (CleanTimer != 0f || IsTooFar(Player, CleanButton.TargetBody))
            return;

        Spread(Player, PlayerByBody(CleanButton.TargetBody));
        CallRpc(CustomRPC.Action, ActionsRPC.FadeBody, this, CleanButton.TargetBody);
        Coroutines.Start(FadeBody(CleanButton.TargetBody));
        LastCleaned = DateTime.UtcNow;

        if (CustomGameOptions.JaniCooldownsLinked)
            LastKilled = DateTime.UtcNow;
    }

    public void Drag()
    {
        if (IsTooFar(Player, DragButton.TargetBody) || CurrentlyDragging || DragTimer != 0f)
            return;

        CurrentlyDragging = DragButton.TargetBody;
        Spread(Player, PlayerByBody(CurrentlyDragging));
        CallRpc(CustomRPC.Action, ActionsRPC.Drag, this, CurrentlyDragging);
        var drag = CurrentlyDragging.gameObject.AddComponent<DragBehaviour>();
        drag.Source = Player;
        drag.Dragged = CurrentlyDragging;
    }

    public void Drop()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.Drop, CurrentlyDragging);
        CurrentlyDragging.gameObject.GetComponent<DragBehaviour>().Destroy();
        CurrentlyDragging = null;
        LastDragged = DateTime.UtcNow;
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        CleanButton.Update("CLEAN", CleanTimer, CustomGameOptions.CleanCd, LastImp && CustomGameOptions.SoloBoost ? -CustomGameOptions.UnderdogKillBonus : 0, true,
            CurrentlyDragging == null);
        DragButton.Update("DRAG", DragTimer, CustomGameOptions.DragCd, true, CurrentlyDragging == null);
        DropButton.Update("DROP", true, CurrentlyDragging != null);
    }
}