namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class HideAndSeek : GameModeRole
{
    public override string FactionName => "Hide And Seek";

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.HideAndSeek;
    }

    public override List<PlayerControl> Team() => [ .. GetLayers<Hunter>().Select(x => x.Player) ];

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        __instance.ReportButton.ToggleVisible(false);
        __instance.SabotageButton.ToggleVisible(false);
        __instance.ImpostorVentButton.ToggleVisible(false);
    }
}