namespace TownOfUsReworked.PlayerLayers.Abilities;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class ButtonBarry : Ability
{
    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static float ButtonCooldown { get; set; } = 25f;

    private bool ButtonUsed { get; set; }
    private CustomButton ButtonButton { get; set; }

    public override UColor Color => ClientOptions.CustomAbColors ? CustomColorManager.ButtonBarry : CustomColorManager.Ability;
    public override string Name => "Button Barry";
    public override LayerEnum Type => LayerEnum.ButtonBarry;
    public override Func<string> Description => () => "- You can call a button from anywhere";

    public override void Init() => ButtonButton = CreateButton(this, "BUTTON", new SpriteName("Button"), AbilityTypes.Targetless, KeybindType.Quarternary, (OnClick)Call, (UsableFunc)Usable,
        new Cooldown(ButtonCooldown));

    private void Call()
    {
        ButtonUsed = true;
        FixExtentions.Fix();
        CallMeeting(Player);
    }

    public bool Usable() => !ButtonUsed;
}