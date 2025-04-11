namespace TownOfUsReworked.PlayerLayers.Roles;

public sealed class Runner : GameModeRole
{
    public override LayerEnum Type { get; } = LayerEnum.Runner;
    public override Func<string> StartText { get; } = () => "Speedrun Tasks To Be The Victor";
    protected override UColor MainColor => CustomColorManager.Runner;
    public override string FactionName => "Task Race";
    public override float VisionRange => GameModeSettings.RunnerVision;
    protected override UColor LayerColor => CustomColorManager.TaskRace;

    protected override void Init()
    {
        base.Init();
        Objectives = () => "- Finish your tasks before the others";
        Alignment = Alignment.TaskRace;
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        __instance.ReportButton.ToggleVisible(false);
        __instance.SabotageButton.ToggleVisible(false);
        __instance.ImpostorVentButton.ToggleVisible(false);
    }

    public override void UponTaskComplete(uint taskId)
    {
        if (TasksLeft == 1)
            Flash(Color);
    }
}