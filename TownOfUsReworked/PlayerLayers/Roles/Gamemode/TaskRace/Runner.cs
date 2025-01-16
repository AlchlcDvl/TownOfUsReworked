namespace TownOfUsReworked.PlayerLayers.Roles;

public class Runner : GameModeRole
{
    public override LayerEnum Type => LayerEnum.Runner;
    public override Func<string> StartText => () => "Speedrun Tasks To Be The Victor";
    public override UColor Color => CustomColorManager.Runner;
    public override string FactionName => "Task Race";

    public override void Init()
    {
        base.Init();
        Objectives = () => "- Finish your tasks before the others";
        Alignment = Alignment.GameModeTaskRace;
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

    public override void CheckWin()
    {
        if (TasksDone)
        {
            WinState = WinLose.TaskRunnerWins;
            Winner = true;
            CallRpc(CustomRPC.WinLose, WinLose.TaskRunnerWins, this);
        }
    }
}