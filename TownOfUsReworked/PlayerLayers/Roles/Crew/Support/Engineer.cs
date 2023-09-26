namespace TownOfUsReworked.PlayerLayers.Roles;

public class Engineer : Crew
{
    public CustomButton FixButton { get; set; }

    public override Color Color => ClientGameOptions.CustomCrewColors ? Colors.Engineer : Colors.Crew;
    public override string Name => "Engineer";
    public override LayerEnum Type => LayerEnum.Engineer;
    public override Func<string> StartText => () => "Just Fix It";
    public override Func<string> Description => () => "- You can fix sabotages at any time from anywhere\n- You can vent";
    public override InspectorResults InspectorResults => InspectorResults.NewLens;

    public Engineer(PlayerControl player) : base(player)
    {
        Alignment = Alignment.CrewSupport;
        FixButton = new(this, "Fix", AbilityTypes.Targetless, "ActionSecondary", Fix, CustomGameOptions.FixCd, CustomGameOptions.MaxFixes);
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        FixButton.Update2("FIX SABOTAGE", condition: Condition());
    }

    public bool Condition()
    {
        var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();
        var dummyActive = system?.dummy.IsActive;
        var active = system?.specials.Any(s => s.IsActive);
        return active == true && dummyActive == false;
    }

    public void Fix()
    {
        FixExtentions.Fix();
        FixButton.StartCooldown(CooldownType.Start);
    }
}