namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Chameleon : Crew
{
    [NumberOption(MultiMenu.LayerSubOptions, 0, 15, 1, ZeroIsInfinity = true)]
    public static Number MaxSwoops { get; set; } = new(5);

    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number SwoopCd { get; set; } = new(25);

    [NumberOption(MultiMenu.LayerSubOptions, 5f, 30f, 1f, Format.Time)]
    public static Number SwoopDur { get; set; } = new(10);

    public CustomButton SwoopButton { get; set; }

    public override UColor Color => ClientOptions.CustomCrewColors ? CustomColorManager.Chameleon : CustomColorManager.Crew;
    public override string Name => "Chameleon";
    public override LayerEnum Type => LayerEnum.Chameleon;
    public override Func<string> StartText => () => "Go Invisible To Stalk Players";
    public override Func<string> Description => () => "- You can turn invisible";

    public override void Init()
    {
        BaseStart();
        Alignment = Alignment.CrewSupport;
        SwoopButton = CreateButton(this, "SWOOP", new SpriteName("Swoop"), AbilityType.Targetless, KeybindType.ActionSecondary, (OnClick)Swoop, new Cooldown(SwoopCd), (EffectVoid)Invis,
            new Duration(SwoopDur), (EffectEndVoid)UnInvis, (EndFunc)EndEffect, MaxSwoops);
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