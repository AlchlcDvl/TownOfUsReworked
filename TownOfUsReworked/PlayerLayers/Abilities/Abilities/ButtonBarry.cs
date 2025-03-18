namespace TownOfUsReworked.PlayerLayers.Abilities;

[LayerHeaderOption(LayerEnum.ButtonBarry)]
public sealed class ButtonBarry : Ability
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number ButtonCd = 25;

    private bool ButtonUsed { get; set; }
    private CustomButton ButtonButton { get; set; }

    public override UColor MainColor => CustomColorManager.ButtonBarry;
    public override LayerEnum Type => LayerEnum.ButtonBarry;
    public override Func<string> Description => () => "- You can call a button from anywhere";

    protected override void Init() => ButtonButton ??= new(this, "BUTTON", new SpriteName("Button"), AbilityTypes.Targetless, KeybindType.Quarternary, (OnClickTargetless)Call, (UsableFunc)Usable,
        new Cooldown(ButtonCd));

    private void Call()
    {
        ButtonUsed = true;
        Fixes.FixCritSabotages();
        CallMeeting(Player);
    }

    private bool Usable() => !ButtonUsed;
}