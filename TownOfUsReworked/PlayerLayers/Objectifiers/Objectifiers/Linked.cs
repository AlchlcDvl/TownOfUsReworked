namespace TownOfUsReworked.PlayerLayers.Objectifiers;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Linked : Objectifier
{
    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool LinkedChat { get; set; } = true;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool LinkedRoles { get; set; } = true;

    public PlayerControl OtherLink { get; set; }

    public override UColor Color => ClientOptions.CustomObjColors ? CustomColorManager.Linked : CustomColorManager.Objectifier;
    public override string Name => "Linked";
    public override string Symbol => "Ψ";
    public override LayerEnum Type => LayerEnum.Linked;
    public override Func<string> Description => () => $"- Help {OtherLink.Data.PlayerName} win";
}