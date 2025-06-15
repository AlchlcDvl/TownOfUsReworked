namespace TownOfUsReworked.PlayerLayers.Abilities;

[LayerHeaderOption(Layer.ButtonBarry)]
public sealed class ButtonBarry : Ability
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number ButtonCd = 25;

    private bool ButtonUsed;
    private CustomButton ButtonButton;

    protected override UColor MainColor => CustomColorManager.ButtonBarry;
    public override Layer Type => Layer.ButtonBarry;
    public override string Description => "- You can call a button from anywhere";

    public override void Init() => ButtonButton ??= new(this, "BUTTON", new SpriteName("Button"), AbilityTypes.Targetless, KeybindType.Quarternary, (OnClickTargetless)Call, (UsableFunc)Usable,
        new Cooldown(ButtonCd));

    private void Call()
    {
        ButtonUsed = true;
        FixUtils.FixCritSabotages();
        CallMeeting(Player);
    }

    private bool Usable() => !ButtonUsed;
}