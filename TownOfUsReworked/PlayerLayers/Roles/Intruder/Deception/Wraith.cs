namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Wraith : Intruder
{
    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number InvisCd { get; set; } = new(25);

    [NumberOption(MultiMenu.LayerSubOptions, 5f, 30f, 1f, Format.Time)]
    public static Number InvisDur { get; set; } = new(10);

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool WraithVent { get; set; } = false;

    public CustomButton InvisButton { get; set; }

    public override UColor Color => ClientOptions.CustomIntColors ? CustomColorManager.Wraith : CustomColorManager.Intruder;
    public override string Name => "Wraith";
    public override LayerEnum Type => LayerEnum.Wraith;
    public override Func<string> StartText => () => "Sneaky Sneaky";
    public override Func<string> Description => () => $"- You can turn invisible\n{CommonAbilities}";

    public override void Init()
    {
        BaseStart();
        Alignment = Alignment.IntruderDecep;
        Data.Role.IntroSound = GetAudio("WraithIntro");
        InvisButton ??= CreateButton(this, "INVISIBILITY", new SpriteName("Invis"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClick)HitInvis, new Cooldown(InvisCd),
            (EffectVoid)Invis, new Duration(InvisDur), (EffectEndVoid)UnInvis, (EndFunc)EndEffect);
    }

    public void Invis() => Utils.Invis(Player, CustomPlayer.Local.Is(Faction.Intruder));

    public void UnInvis() => DefaultOutfit(Player);

    public void HitInvis()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, InvisButton);
        InvisButton.Begin();
    }

    public bool EndEffect() => Dead;
}