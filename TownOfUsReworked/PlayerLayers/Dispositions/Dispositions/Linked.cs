namespace TownOfUsReworked.PlayerLayers.Dispositions;

[LayerHeaderOption(Layer.Linked)]
public sealed class Linked : Paired
{
    [ToggleOption]
    public static bool LinkedChat = true;

    [ToggleOption]
    private static bool LinkedRoles = true;

    protected override UColor MainColor => CustomColorManager.Linked;
    public override string Symbol => "Ψ";
    public override Layer Type => Layer.Linked;
    public override string Description => $"- Help {Other.name} win";
    protected override bool RevealRole => LinkedRoles;
    protected override ChatChannel Channel => ChatChannel.Linked;
}