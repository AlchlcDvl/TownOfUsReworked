namespace TownOfUsReworked.PlayerLayers.Objectifiers;

public class Linked : Objectifier
{
    public PlayerControl OtherLink { get; set; }

    public override UColor Color => ClientGameOptions.CustomObjColors ? CustomColorManager.Linked : CustomColorManager.Objectifier;
    public override string Name => "Linked";
    public override string Symbol => "Ψ";
    public override LayerEnum Type => LayerEnum.Linked;
    public override Func<string> Description => () => $"- Help {OtherLink.name} win";

    public Linked() : base() {}

    public override PlayerLayer Start(PlayerControl player)
    {
        SetPlayer(player);
        return this;
    }
}