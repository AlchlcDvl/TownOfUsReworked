namespace TownOfUsReworked.PlayerLayers.Roles;

public class Engineer : Crew
{
    public CustomButton FixButton { get; set; }
    public int UsesLeft { get; set; }
    public bool ButtonUsable => UsesLeft > 0;
    public DateTime LastFixed { get; set; }
    public float Timer => ButtonUtils.Timer(Player, LastFixed, CustomGameOptions.FixCd);

    public override Color Color => ClientGameOptions.CustomCrewColors ? Colors.Engineer : Colors.Crew;
    public override string Name => "Engineer";
    public override LayerEnum Type => LayerEnum.Engineer;
    public override Func<string> StartText => () => "Just Fix It";
    public override Func<string> Description => () => "- You can fix sabotages at any time from anywhere\n- You can vent";
    public override InspectorResults InspectorResults => InspectorResults.NewLens;

    public Engineer(PlayerControl player) : base(player)
    {
        Alignment = Alignment.CrewSupport;
        UsesLeft = CustomGameOptions.MaxFixes;
        FixButton = new(this, "Fix", AbilityTypes.Effect, "ActionSecondary", Fix, true);
    }

    public void Fix()
    {
        if (!ButtonUsable || Timer != 0f)
            return;

        var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();

        if (system == null)
            return;

        var dummyActive = system.dummy.IsActive;
        var sabActive = system.specials.Any(s => s.IsActive);

        if (!sabActive || dummyActive)
            return;

        UsesLeft--;
        LastFixed = DateTime.UtcNow;
        FixExtentions.Fix();
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();
        var dummyActive = system?.dummy.IsActive;
        var active = system?.specials.Any(s => s.IsActive);
        var condition = active == true && dummyActive == false;
        FixButton.Update("FIX", Timer, CustomGameOptions.FixCd, UsesLeft, condition, ButtonUsable);
    }
}