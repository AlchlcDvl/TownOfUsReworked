namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Runner)]
public sealed class Runner : GameMode
{
    [NumberOption(1f, 2f, 0.05f, Format.Multiplier)]
    public static Number RunnerVision = 1.5f;

    [ToggleOption]
    public static bool RunnerFlashlight = false;

    public override LayerEnum Type => LayerEnum.Runner;
    public override Func<string> StartText { get; } = () => "Speedrun Tasks To Be The Victor";
    protected override UColor MainColor => CustomColorManager.Runner;
    public override string FactionName => "Task Race";
    protected override UColor LayerColor => CustomColorManager.TaskRace;

    protected override void Init()
    {
        base.Init();
        Objectives = () => "- Finish your tasks before the others";
        Alignment = Alignment.TaskRace;
    }

    public override void UponTaskComplete(uint taskId)
    {
        if (TasksLeft == 1)
            Flash(Color);
    }
}