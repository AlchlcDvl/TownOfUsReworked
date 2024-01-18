namespace TownOfUsReworked.PlayerLayers.Roles;

public class Chameleon : Crew
{
    public CustomButton SwoopButton { get; set; }

    public override UColor Color => ClientGameOptions.CustomCrewColors ? CustomColorManager.Chameleon : CustomColorManager.Crew;
    public override string Name => "Chameleon";
    public override LayerEnum Type => LayerEnum.Chameleon;
    public override Func<string> StartText => () => "Go Invisible To Stalk Players";
    public override Func<string> Description => () => "- You can turn invisible";

    public Chameleon(PlayerControl player) : base(player) => SwoopButton = new(this, "Swoop", AbilityTypes.Targetless, "ActionSecondary", Swoop, CustomGameOptions.SwoopCd,
        CustomGameOptions.SwoopDur, Invis, UnInvis, CustomGameOptions.MaxSwoops);

    public void Invis() => Utils.Invis(Player);

    public void UnInvis() => DefaultOutfit(Player);

    public void Swoop()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction1, SwoopButton);
        SwoopButton.Begin();
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        SwoopButton.Update2("SWOOP");
    }

    public override void TryEndEffect() => SwoopButton.Update3(IsDead);
}