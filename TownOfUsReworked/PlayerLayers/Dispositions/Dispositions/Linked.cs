namespace TownOfUsReworked.PlayerLayers.Dispositions;

[LayerHeaderOption(LayerEnum.Linked)]
public sealed class Linked : Paired
{
    [ToggleOption]
    public static bool LinkedChat = true;

    [ToggleOption]
    private static bool LinkedRoles = true;

    protected override UColor MainColor => CustomColorManager.Linked;
    public override string Symbol => "Ψ";
    public override LayerEnum Type => LayerEnum.Linked;
    public override string Description => $"- Help {Other.name} win";
    protected override bool RevealRole => LinkedRoles;
    protected override ChatChannel Channel => ChatChannel.Linked;
}