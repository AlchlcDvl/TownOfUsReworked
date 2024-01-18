namespace TownOfUsReworked.PlayerLayers.Objectifiers;

public class Rivals : Objectifier
{
    public PlayerControl OtherRival { get; set; }
    private bool RivalDead => OtherRival == null || OtherRival.HasDied();
    private bool IsDeadRival => Player == null || Player.HasDied();
    public bool IsWinningRival =>  RivalDead && !IsDeadRival;

    public override UColor Color => ClientGameOptions.CustomObjColors ? CustomColorManager.Rivals : CustomColorManager.Objectifier;
    public override string Name => "Rivals";
    public override string Symbol => "α";
    public override LayerEnum Type => LayerEnum.Rivals;
    public override Func<string> Description => () => $"- Get {OtherRival.name} killed and then live to the final 2";

    public Rivals(PlayerControl player) : base(player) {}
}