namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class HideAndSeek : Role
{
    public override string FactionName => "Hide And Seek";

    public override void Init()
    {
        base.Init();
        Faction = Faction.GameMode;
        Alignment = Alignment.GameModeHideAndSeek;
    }

    public override List<PlayerControl> Team()
    {
        var team = new List<PlayerControl>();
        team.AddRange(GetLayers<Hunter>().Select(x => x.Player));
        return team;
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        __instance.ReportButton.ToggleVisible(false);
        __instance.SabotageButton.ToggleVisible(false);
        __instance.ImpostorVentButton.ToggleVisible(false);
    }
}