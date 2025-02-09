namespace TownOfUsReworked.PlayerLayers.Modifiers;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class VIP : Modifier
{
    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool VIPKnows { get; set; } = true;

    public override UColor Color => ClientOptions.CustomModColors ? CustomColorManager.VIP : CustomColorManager.Modifier;
    public override string Name => "VIP";
    public override LayerEnum Type => LayerEnum.VIP;
    public override Func<string> Description => () => "- Your death will alert everyone and will have an arrow pointing at your body";
    public override bool Hidden => !VIPKnows && !Dead;
}