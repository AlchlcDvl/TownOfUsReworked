namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(Layer.Runner)]
public sealed class Runner : GameMode
{
    [NumberOption(1f, 2f, 0.05f, Format.Multiplier)]
    public static Number RunnerVision = 1.5f;

    [ToggleOption]
    public static bool RunnerFlashlight = false;

    public override Layer Type => Layer.Runner;
    public override string StartText => "Speedrun Tasks To Be The Victor";
    protected override UColor MainColor => CustomColorManager.Runner;
    public override string FactionName => "Task Race";
    public override Alignment Alignment => Alignment.TaskRace;

    public override void Init() => Objectives = () => "- Finish your tasks before the others";

    public override void UponTaskComplete(uint taskId)
    {
        if (TasksLeft == 1)
            Flash(Color);
    }

    protected override void CheckWin(HashSet<byte> winnerIds)
    {
        if (!TasksDone)
            return;

        WinState = WinLose.TaskRunnerWins;
        winnerIds.Add(PlayerId);
    }
}