namespace TownOfUsReworked.PlayerLayers.Dispositions;

[LayerHeaderOption(LayerEnum.Fanatic)]
public sealed class Fanatic : FactionChanger
{
    [ToggleOption]
    private static bool FanaticKnows = true;

    [ToggleOption]
    private static bool FanaticColourSwap = false;

    protected override UColor MainColor => CustomColorManager.Fanatic;
    public override string Symbol => "♠";
    public override LayerEnum Type => LayerEnum.Fanatic;
    public override string Description => !Turned ? "- Get attacked by either an <#FF1919FF>Intruder</color> or a <#008000FF>Syndicate</color> to join their side" : "";
    public override bool Hidden => !FanaticKnows && !Turned && !Dead;
    public override bool SnitchReveals => Snitch.SnitchSeesFanatic;
    public override bool RevealerReveals => Revealer.RevealerRevealsFanatic;
    public override bool SheriffSwap => FanaticColourSwap;
}