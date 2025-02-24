namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Wraith : Intruder
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number InvisCd = 25;

    [NumberOption(5f, 30f, 1f, Format.Time)]
    public static Number InvisDur = 10;

    [ToggleOption]
    public static bool WraithVent = false;

    private CustomButton InvisButton { get; set; }

    public override UColor Color => ClientOptions.CustomIntColors ? CustomColorManager.Wraith : FactionColor;
    public override LayerEnum Type => LayerEnum.Wraith;
    public override Func<string> StartText => () => "Sneaky Sneaky";
    public override Func<string> Description => () => $"- You can turn invisible\n{CommonAbilities}";

    protected override void Init()
    {
        base.Init();
        Alignment = Alignment.Deception;
        InvisButton ??= new(this, "INVISIBILITY", new SpriteName("Invis"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClickTargetless)HitInvis, new Cooldown(InvisCd), (EffectVoid)Invis,
            new Duration(InvisDur), (EffectEndVoid)UnInvis, (EndFunc)EndEffect);
    }

    private void Invis() => MiscUtils.Invis(Player, CustomPlayer.Local.Is(Faction.Intruder));

    private void UnInvis() => DefaultOutfit(Player);

    private void HitInvis()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, InvisButton);
        InvisButton.Begin();
    }

    private bool EndEffect() => Dead;
}