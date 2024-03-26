namespace TownOfUsReworked.PlayerLayers.Roles;

public class Chameleon : Crew
{
    public CustomButton SwoopButton { get; set; }

    public override UColor Color => ClientGameOptions.CustomCrewColors ? CustomColorManager.Chameleon : CustomColorManager.Crew;
    public override string Name => "Chameleon";
    public override LayerEnum Type => LayerEnum.Chameleon;
    public override Func<string> StartText => () => "Go Invisible To Stalk Players";
    public override Func<string> Description => () => "- You can turn invisible";

    public override void Init()
    {
        BaseStart();
        Alignment = Alignment.CrewSupport;
        SwoopButton = CreateButton(this, "SWOOP", new SpriteName("Swoop"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClick)Swoop, new Cooldown(CustomGameOptions.SwoopCd),
            new Duration(CustomGameOptions.SwoopDur), (EffectVoid)Invis, (EffectEndVoid)UnInvis, (EndFunc)EndEffect, CustomGameOptions.MaxSwoops);
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