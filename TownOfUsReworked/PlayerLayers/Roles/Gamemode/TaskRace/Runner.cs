namespace TownOfUsReworked.PlayerLayers.Roles;

public class Runner : Role
{
    public override string Name => "Runner";
    public override LayerEnum Type => LayerEnum.Runner;
    public override Func<string> StartText => () => "Speedrun Tasks To Be The Victor";
    public override Faction BaseFaction => Faction = Faction.GameMode;
    public override Color Color => Colors.Runner;
    public override string FactionName => "Task Race";

    public Runner(PlayerControl player) : base(player)
    {
        FactionColor = Colors.TaskRace;
        Objectives = () => "- Finish your tasks before the others";
        Player.Data.SetImpostor(false);
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        __instance.ReportButton.gameObject.SetActive(false);
        __instance.SabotageButton.gameObject.SetActive(false);
        __instance.ImpostorVentButton.gameObject.SetActive(false);
    }
}