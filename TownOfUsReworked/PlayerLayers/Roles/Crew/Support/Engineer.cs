namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Engineer : Crew
{
    [NumberOption(MultiMenu.LayerSubOptions, 0, 15, 1, ZeroIsInfinity = true)]
    public static Number MaxFixes { get; set; } = new(5);

    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number FixCd { get; set; } = new(5);

    public CustomButton FixButton { get; set; }

    public override UColor Color => ClientOptions.CustomCrewColors ? CustomColorManager.Engineer : FactionColor;
    public override string Name => "Engineer";
    public override LayerEnum Type => LayerEnum.Engineer;
    public override Func<string> StartText => () => "Just Fix It";
    public override Func<string> Description => () => "- You can fix sabotages at any time from anywhere\n- You can vent";

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.CrewSupport;
        FixButton ??= new(this, "FIX SABOTAGE", new SpriteName("Fix"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClickTargetless)Fix, new Cooldown(FixCd), MaxFixes,
            (ConditionFunc)Condition);
    }

    public bool Condition() => Ship().Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>().AnyActive;

    public void Fix()
    {
        FixExtentions.Fix();
        FixButton.StartCooldown(CooldownType.Reset);
    }
}