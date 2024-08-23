namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu2.LayerSubOptions)]
public class Chameleon : Crew
{
    [NumberOption(MultiMenu2.LayerSubOptions, 1, 15, 1)]
    public static int MaxSwoops { get; set; } = 5;

    [NumberOption(MultiMenu2.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static float SwoopCd { get; set; } = 25f;

    [NumberOption(MultiMenu2.LayerSubOptions, 5f, 30f, 1f, Format.Time)]
    public static float SwoopDur { get; set; } = 10f;

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
        SwoopButton = CreateButton(this, "SWOOP", new SpriteName("Swoop"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClick)Swoop, new Cooldown(SwoopCd), (EffectVoid)Invis,
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