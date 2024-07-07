namespace TownOfUsReworked.PlayerLayers.Objectifiers;

public class Rivals : Objectifier
{
    public PlayerControl OtherRival { get; set; }
    public bool IsWinningRival =>  OtherRival.HasDied() && !Player.HasDied();

    public override UColor Color => ClientGameOptions.CustomObjColors ? CustomColorManager.Rivals : CustomColorManager.Objectifier;
    public override string Name => "Rivals";
    public override string Symbol => "α";
    public override LayerEnum Type => LayerEnum.Rivals;
    public override Func<string> Description => () => OtherRival.HasDied() ? "- Live to the final 2" : $"- Get {OtherRival.Data.PlayerName} killed";
}