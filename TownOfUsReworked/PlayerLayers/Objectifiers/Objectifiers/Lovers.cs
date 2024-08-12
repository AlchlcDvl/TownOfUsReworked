namespace TownOfUsReworked.PlayerLayers.Objectifiers;

[HeaderOption(MultiMenu2.LayerSubOptions)]
public class Lovers : Objectifier
{
    public PlayerControl OtherLover { get; set; }
    public bool LoversAlive => !Player.HasDied() && !OtherLover.HasDied();

    public override UColor Color => ClientOptions.CustomObjColors ? CustomColorManager.Lovers : CustomColorManager.Objectifier;
    public override string Name => "Lovers";
    public override string Symbol => "♥";
    public override LayerEnum Type => LayerEnum.Lovers;
    public override Func<string> Description => () => $"- Live to the final 3 with {OtherLover.Data.PlayerName}";
}