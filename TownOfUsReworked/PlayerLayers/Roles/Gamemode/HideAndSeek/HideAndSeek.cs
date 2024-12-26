namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class HideAndSeek : Role
{
    public override Faction BaseFaction => Faction.GameMode;
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
        __instance.ReportButton.gameObject.SetActive(false);
        __instance.SabotageButton.gameObject.SetActive(false);
        __instance.ImpostorVentButton.gameObject.SetActive(false);
    }
}