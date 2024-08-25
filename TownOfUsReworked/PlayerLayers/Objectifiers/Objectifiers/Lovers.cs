namespace TownOfUsReworked.PlayerLayers.Objectifiers;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Lovers : Objectifier
{
    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool BothLoversDie { get; set; } = true;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool LoversChat { get; set; } = true;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool LoversRoles { get; set; } = true;

    public PlayerControl OtherLover { get; set; }
    public bool LoversAlive => !Player.HasDied() && !OtherLover.HasDied();

    public override UColor Color => ClientOptions.CustomObjColors ? CustomColorManager.Lovers : CustomColorManager.Objectifier;
    public override string Name => "Lovers";
    public override string Symbol => "♥";
    public override LayerEnum Type => LayerEnum.Lovers;
    public override Func<string> Description => () => $"- Live to the final 3 with {OtherLover.Data.PlayerName}";
}