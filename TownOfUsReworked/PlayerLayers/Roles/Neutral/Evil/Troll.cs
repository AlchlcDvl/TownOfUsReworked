namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Troll : Neutral
{
    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number InteractCd { get; set; } = new(25);

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool TrollVent { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool TrollSwitchVent { get; set; } = false;

    public bool Killed => DeathReason is not (DeathReasonEnum.Alive or DeathReasonEnum.Ejected or DeathReasonEnum.Guessed or DeathReasonEnum.Revived);
    public CustomButton InteractButton { get; set; }

    public override UColor Color => ClientOptions.CustomNeutColors ? CustomColorManager.Troll : CustomColorManager.Neutral;
    public override string Name => "Troll";
    public override LayerEnum Type => LayerEnum.Troll;
    public override Func<string> StartText => () => "Troll Everyone With Your Death";
    public override Func<string> Description => () => "- You can interact with players\n- Your interactions do nothing except spread infection and possibly kill you via touch sensitive" +
        " roles\n- If you are killed, you will also kill your killer";
    public override AttackEnum AttackVal => AttackEnum.Unstoppable;

    public override void Init()
    {
        BaseStart();
        Alignment = Alignment.NeutralEvil;
        Objectives = () => Killed ? "- You have successfully trolled someone" : "- Get killed";
        InteractButton ??= CreateButton(this, new SpriteName("Placeholder"), AbilityTypes.Alive, KeybindType.ActionSecondary, (OnClick)Interact, new Cooldown(InteractCd), "INTERACT");
        Data.Role.IntroSound = GetAudio("TrollIntro");
    }

    public void Interact() => InteractButton.StartCooldown(Interactions.Interact(Player, InteractButton.GetTarget<PlayerControl>()));
}