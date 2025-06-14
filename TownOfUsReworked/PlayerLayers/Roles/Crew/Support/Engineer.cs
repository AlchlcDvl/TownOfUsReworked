namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Engineer)]
public sealed class Engineer : CSupport
{
    [NumberOption(0, 15, 1, zeroIsInf: true)]
    public static Number MaxFixes = 5;

    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number FixCd = 5;

    private CustomButton FixButton;

    protected override UColor MainColor => CustomColorManager.Engineer;
    public override LayerEnum Type => LayerEnum.Engineer;
    public override string StartText => "Just Fix It";
    public override string Description => "- You can fix sabotages at any time from anywhere\n- You can vent";

    public override void Init()
    {
        base.Init();
        FixButton ??= new(this, "FIX SABOTAGE", new SpriteName("Fix"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClickTargetless)Fix, new Cooldown(FixCd), MaxFixes,
            (ConditionFunc)Condition);
    }

    public static bool Condition() => Ship().Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>().AnyActive;

    private void Fix()
    {
        FixUtils.Fix();
        FixButton.StartCooldown();
    }
}