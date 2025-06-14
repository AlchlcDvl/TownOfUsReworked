namespace TownOfUsReworked.PlayerLayers.Dispositions;

[LayerHeaderOption(LayerEnum.Overlord)]
public sealed class Overlord : Disposition
{
    [ToggleOption]
    private static bool OverlordKnows = true;

    [NumberOption(1, 20, 1)]
    public static Number OverlordMeetingWinCount = 3;

    protected override UColor MainColor => CustomColorManager.Overlord;
    public override string Symbol => "β";
    public override LayerEnum Type => LayerEnum.Overlord;
    public override string Description => $"- Stay alive for {OverlordMeetingWinCount} rounds";
    public override bool Hidden => !OverlordKnows && !Dead;

    protected override void CheckWin(HashSet<byte> winnerIds)
    {
        if (!Alive || MeetingPatches.MeetingCount < OverlordMeetingWinCount)
            return;

        WinState = WinLose.OverlordWins;
        winnerIds.Add(PlayerId);
    }
}