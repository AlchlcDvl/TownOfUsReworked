namespace TownOfUsReworked.PlayerLayers.Roles;

public class Wraith : Intruder
{
    public CustomButton InvisButton { get; set; }

    public override UColor Color => ClientGameOptions.CustomIntColors ? CustomColorManager.Wraith : CustomColorManager.Intruder;
    public override string Name => "Wraith";
    public override LayerEnum Type => LayerEnum.Wraith;
    public override Func<string> StartText => () => "Sneaky Sneaky";
    public override Func<string> Description => () => $"- You can turn invisible\n{CommonAbilities}";

    public override void Init()
    {
        BaseStart();
        Alignment = Alignment.IntruderDecep;
        InvisButton = CreateButton(this, "INVISIBILITY", new SpriteName("Invis"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClick)HitInvis, new Cooldown(CustomGameOptions.InvisCd),
            (EffectVoid)Invis, new Duration(CustomGameOptions.InvisDur), (EffectEndVoid)UnInvis, (EndFunc)EndEffect);
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