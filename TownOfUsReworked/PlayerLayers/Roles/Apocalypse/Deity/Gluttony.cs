namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Gluttony)]
public sealed class Gluttony : Deity
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number HungerCd = 25;

    [NumberOption(5f, 30f, 1f, Format.Time)]
    public static Number HungerDur = 10;

    [ToggleOption]
    private static bool GlutVent = true;

    private CustomButton HungerButton { get; set; }

    protected override UColor MainColor => CustomColorManager.Gluttony;
    public override LayerEnum Type => LayerEnum.Gluttony;
    public override Func<string> Description => () => "- You can initiate an insatiable hunger into people, causing them to have the need to eat someone else" + CommonAbilities;
    public override bool CanVent => base.CanVent && GlutVent;

    protected override void Init()
    {
        base.Init();
        HungerButton ??= new(this, new SpriteName("Hunger"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClickTargetless)CauseHunger, "Hunger", new Cooldown(HungerCd),
            new Duration(HungerDur));
    }

    private void CauseHunger()
    {
        HungerButton.StartCooldown();
    }
}