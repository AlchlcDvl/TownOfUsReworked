namespace TownOfUsReworked.PlayerLayers.Objectifiers;

[HeaderOption(MultiMenu2.LayerSubOptions)]
public class Linked : Objectifier
{
    public PlayerControl OtherLink { get; set; }

    public override UColor Color => ClientOptions.CustomObjColors ? CustomColorManager.Linked : CustomColorManager.Objectifier;
    public override string Name => "Linked";
    public override string Symbol => "Ψ";
    public override LayerEnum Type => LayerEnum.Linked;
    public override Func<string> Description => () => $"- Help {OtherLink.Data.PlayerName} win";
}