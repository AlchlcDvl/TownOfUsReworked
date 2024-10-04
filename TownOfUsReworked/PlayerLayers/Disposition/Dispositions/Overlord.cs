namespace TownOfUsReworked.PlayerLayers.Dispositions;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Overlord : Disposition
{
    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool OverlordKnows { get; set; } = true;

    [NumberOption(MultiMenu.LayerSubOptions, 1, 20, 1)]
    public static Number OverlordMeetingWinCount { get; set; } = new(3);

    public override UColor Color => ClientOptions.CustomDispColors ? CustomColorManager.Overlord : CustomColorManager.Disposition;
    public override string Name => "Overlord";
    public override string Symbol => "β";
    public override LayerEnum Type => LayerEnum.Overlord;
    public override Func<string> Description => () => $"- Stay alive for {OverlordMeetingWinCount} rounds";
    public override bool Hidden => !OverlordKnows && !Dead;
}