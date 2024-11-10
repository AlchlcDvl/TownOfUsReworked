namespace TownOfUsReworked.PlayerLayers.Roles;

public class Runner : Role
{
    public override string Name => "Runner";
    public override LayerEnum Type => LayerEnum.Runner;
    public override Func<string> StartText => () => "Speedrun Tasks To Be The Victor";
    public override Faction BaseFaction => Faction.GameMode;
    public override UColor Color => CustomColorManager.Runner;
    public override string FactionName => "Task Race";

    public override void Init()
    {
        base.Init();
        Faction = Faction.GameMode;
        FactionColor = CustomColorManager.TaskRace;
        Objectives = () => "- Finish your tasks before the others";
        Alignment = Alignment.GameModeTaskRace;
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        __instance.ReportButton.gameObject.SetActive(false);
        __instance.SabotageButton.gameObject.SetActive(false);
        __instance.ImpostorVentButton.gameObject.SetActive(false);
    }

    public override void UponTaskComplete(uint taskId)
    {
        if (TasksLeft == 1)
            Flash(Color);
    }
}