namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public sealed class Engineer : Crew
{
    [NumberOption(0, 15, 1, zeroIsInf: true)]
    public static Number MaxFixes = 5;

    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number FixCd = 5;

    private CustomButton FixButton { get; set; }

    public override UColor Color => ClientOptions.CustomCrewColors ? CustomColorManager.Engineer : FactionColor;
    public override LayerEnum Type => LayerEnum.Engineer;
    public override Func<string> StartText => () => "Just Fix It";
    public override Func<string> Description => () => "- You can fix sabotages at any time from anywhere\n- You can vent";

    protected override void Init()
    {
        base.Init();
        Alignment = Alignment.Support;
        FixButton ??= new(this, "FIX SABOTAGE", new SpriteName("Fix"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClickTargetless)Fix, new Cooldown(FixCd), MaxFixes,
            (ConditionFunc)Condition);
    }

    public static bool Condition() => Ship().Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>().AnyActive;

    private void Fix()
    {
        Fixes.Fix();
        FixButton.StartCooldown();
    }
}