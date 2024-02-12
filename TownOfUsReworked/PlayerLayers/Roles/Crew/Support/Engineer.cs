namespace TownOfUsReworked.PlayerLayers.Roles;

public class Engineer : Crew
{
    public CustomButton FixButton { get; set; }

    public override UColor Color => ClientGameOptions.CustomCrewColors ? CustomColorManager.Engineer : CustomColorManager.Crew;
    public override string Name => "Engineer";
    public override LayerEnum Type => LayerEnum.Engineer;
    public override Func<string> StartText => () => "Just Fix It";
    public override Func<string> Description => () => "- You can fix sabotages at any time from anywhere\n- You can vent";

    public Engineer() : base() {}

    public override PlayerLayer Start(PlayerControl player)
    {
        SetPlayer(player);
        BaseStart();
        Alignment = Alignment.CrewSupport;
        FixButton = new(this, "Fix", AbilityTypes.Targetless, "ActionSecondary", Fix, CustomGameOptions.FixCd, CustomGameOptions.MaxFixes);
        Data.Role.IntroSound = GetAudio("EngineerIntro");
        return this;
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        FixButton.Update2("FIX SABOTAGE", CustomGameOptions.MaxFixes > 0, Ship.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>().AnyActive);
    }

    public void Fix()
    {
        FixExtentions.Fix();
        FixButton.StartCooldown(CooldownType.Start);
    }
}