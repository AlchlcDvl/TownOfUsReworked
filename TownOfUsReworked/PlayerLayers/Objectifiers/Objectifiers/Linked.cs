namespace TownOfUsReworked.PlayerLayers.Objectifiers;

public class Linked : Objectifier
{
    public PlayerControl OtherLink { get; set; }

    public override Color32 Color => ClientGameOptions.CustomObjColors ? Colors.Linked : Colors.Objectifier;
    public override string Name => "Linked";
    public override string Symbol => "Ψ";
    public override LayerEnum Type => LayerEnum.Linked;
    public override Func<string> Description => () => $"- Help {OtherLink.name} win";

    public Linked(PlayerControl player) : base(player) {}
}