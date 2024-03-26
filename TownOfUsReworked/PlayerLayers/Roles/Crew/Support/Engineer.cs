namespace TownOfUsReworked.PlayerLayers.Roles;

public class Engineer : Crew
{
    public CustomButton FixButton { get; set; }

    public override UColor Color => ClientGameOptions.CustomCrewColors ? CustomColorManager.Engineer : CustomColorManager.Crew;
    public override string Name => "Engineer";
    public override LayerEnum Type => LayerEnum.Engineer;
    public override Func<string> StartText => () => "Just Fix It";
    public override Func<string> Description => () => "- You can fix sabotages at any time from anywhere\n- You can vent";

    public override void Init()
    {
        BaseStart();
        Alignment = Alignment.CrewSupport;
        FixButton = CreateButton(this, "FIX SABOTAGE", new SpriteName("Fix"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClick)Fix, new Cooldown(CustomGameOptions.FixCd),
            CustomGameOptions.MaxFixes, (ConditionFunc)Condition);
        Data.Role.IntroSound = GetAudio("EngineerIntro");
    }

    public bool Condition() => Ship.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>().AnyActive;

    public void Fix()
    {
        FixExtentions.Fix();
        FixButton.StartCooldown(CooldownType.Reset);
    }
}