namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Gluttony)]
public sealed class Gluttony : Deity
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number RotCd = 25;

    [NumberOption(2, 10, 1)]
    public static Number MaxFoodPreTransformation = 4;

    [ToggleOption]
    private static bool GlutVent = true;

    [NumberOption(10f, 1200f, 5f, Format.Time)]
    public static Number CycleDur = 30;

    private CustomButton RotButton { get; set; }

    protected override UColor MainColor => CustomColorManager.Gluttony;
    public override LayerEnum Type => LayerEnum.Gluttony;
    public override Func<string> Description => () => "- You can initiate an insatiable hunger into people, causing them to have the need to eat someone else" + CommonAbilities;
    public override bool CanVent => base.CanVent && GlutVent;

    protected override void Init()
    {
        base.Init();
        RotButton ??= new(this, new SpriteName("Hunger"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClickTargetless)CauseHunger, "Hunger", new Cooldown(RotCd));
    }

    private void CauseHunger()
    {
        RotButton.StartCooldown();
    }
}