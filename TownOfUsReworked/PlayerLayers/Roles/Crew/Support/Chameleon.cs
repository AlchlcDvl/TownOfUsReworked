namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Chameleon : Crew
{
    [NumberOption(0, 15, 1, zeroIsInf: true)]
    public static Number MaxSwoops = 5;

    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number SwoopCd = 25;

    [NumberOption(5f, 30f, 1f, Format.Time)]
    public static Number SwoopDur = 10;

    public CustomButton SwoopButton { get; set; }

    public override UColor Color => ClientOptions.CustomCrewColors ? CustomColorManager.Chameleon : FactionColor;
    public override LayerEnum Type => LayerEnum.Chameleon;
    public override Func<string> StartText => () => "Go Invisible To Stalk Players";
    public override Func<string> Description => () => "- You can turn invisible";

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.Support;
        SwoopButton ??= new(this, "SWOOP", new SpriteName("Swoop"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClickTargetless)Swoop, new Cooldown(SwoopCd), (EffectVoid)Invis,
            MaxSwoops, new Duration(SwoopDur), (EffectEndVoid)UnInvis, (EndFunc)EndEffect);
    }

    public void Invis() => Utils.Invis(Player);

    public void UnInvis() => DefaultOutfit(Player);

    public void Swoop()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, SwoopButton);
        SwoopButton.Begin();
    }

    public bool EndEffect() => Dead;
}