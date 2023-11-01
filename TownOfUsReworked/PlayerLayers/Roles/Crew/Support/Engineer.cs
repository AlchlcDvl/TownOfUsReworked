namespace TownOfUsReworked.PlayerLayers.Roles;

public class Engineer : Crew
{
    public CustomButton FixButton { get; set; }

    public override Color Color => ClientGameOptions.CustomCrewColors ? Colors.Engineer : Colors.Crew;
    public override string Name => "Engineer";
    public override LayerEnum Type => LayerEnum.Engineer;
    public override Func<string> StartText => () => "Just Fix It";
    public override Func<string> Description => () => "- You can fix sabotages at any time from anywhere\n- You can vent";

    public Engineer(PlayerControl player) : base(player)
    {
        Alignment = Alignment.CrewSupport;
        FixButton = new(this, "Fix", AbilityTypes.Targetless, "ActionSecondary", Fix, CustomGameOptions.FixCd, CustomGameOptions.MaxFixes);
        player.Data.Role.IntroSound = GetAudio("EngineerIntro");
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        FixButton.Update2("FIX SABOTAGE", condition: Condition());
    }

    public bool Condition() => ShipStatus.Instance.Systems[SystemTypes.Sabotage].TryCast<SabotageSystemType>()?.AnyActive == true;

    public void Fix()
    {
        FixExtentions.Fix();
        FixButton.StartCooldown(CooldownType.Start);
    }
}