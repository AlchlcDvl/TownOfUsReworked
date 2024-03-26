namespace TownOfUsReworked.PlayerLayers.Objectifiers;

public class Lovers : Objectifier
{
    public PlayerControl OtherLover { get; set; }
    private bool LoverDead => OtherLover == null || OtherLover.HasDied();
    private bool IsDeadLover => Player == null || Player.HasDied();
    public bool LoversAlive => !IsDeadLover && !LoverDead;

    public override UColor Color => ClientGameOptions.CustomObjColors ? CustomColorManager.Lovers : CustomColorManager.Objectifier;
    public override string Name => "Lovers";
    public override string Symbol => "♥";
    public override LayerEnum Type => LayerEnum.Lovers;
    public override Func<string> Description => () => $"- Live to the final 3 with {OtherLover.Data.PlayerName}";
}