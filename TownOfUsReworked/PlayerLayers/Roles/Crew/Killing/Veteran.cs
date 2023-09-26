namespace TownOfUsReworked.PlayerLayers.Roles;

public class Veteran : Crew
{
    public CustomButton AlertButton { get; set; }

    public override Color Color => ClientGameOptions.CustomCrewColors ? Colors.Veteran : Colors.Crew;
    public override string Name => "Veteran";
    public override LayerEnum Type => LayerEnum.Veteran;
    public override Func<string> StartText => () => "Alert To Kill Anyone Who Dares To Touch You";
    public override Func<string> Description => () => "- You can go on alert\n- When on alert, you will kill whoever interacts with you";
    public override InspectorResults InspectorResults => InspectorResults.IsCold;

    public Veteran(PlayerControl player) : base(player)
    {
        Alignment = Alignment.CrewKill;
        AlertButton = new(this, "Alert", AbilityTypes.Targetless, "ActionSecondary", Alert, CustomGameOptions.AlertCd, CustomGameOptions.AlertDur, CustomGameOptions.MaxAlerts);
    }

    public void Alert()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction1, AlertButton);
        AlertButton.Begin();
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        AlertButton.Update2("ALERT");
    }

    public override void TryEndEffect() => AlertButton.Update3(IsDead);
}