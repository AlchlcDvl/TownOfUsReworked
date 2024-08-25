namespace TownOfUsReworked.PlayerLayers.Objectifiers;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Overlord : Objectifier
{
    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool OverlordKnows { get; set; } = true;

    [NumberOption(MultiMenu.LayerSubOptions, 1, 20, 1)]
    public static int OverlordMeetingWinCount { get; set; } = 2;

    public override UColor Color => ClientOptions.CustomObjColors ? CustomColorManager.Overlord : CustomColorManager.Objectifier;
    public override string Name => "Overlord";
    public override string Symbol => "β";
    public override LayerEnum Type => LayerEnum.Overlord;
    public override Func<string> Description => () => $"- Stay alive for {OverlordMeetingWinCount} rounds";
    public override bool Hidden => !OverlordKnows && !Dead;
}