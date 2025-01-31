namespace TownOfUsReworked.PlayerLayers.Dispositions;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Overlord : Disposition
{
    [ToggleOption]
    public static bool OverlordKnows = true;

    [NumberOption(1, 20, 1)]
    public static Number OverlordMeetingWinCount = 3;

    public override UColor Color => ClientOptions.CustomDispColors ? CustomColorManager.Overlord : CustomColorManager.Disposition;
    public override string Symbol => "β";
    public override LayerEnum Type => LayerEnum.Overlord;
    public override Func<string> Description => () => $"- Stay alive for {OverlordMeetingWinCount} rounds";
    public override bool Hidden => !OverlordKnows && !Dead;

    public override void CheckWin()
    {
        if (Alive && OverlordWins())
        {
            WinState = WinLose.OverlordWins;
            GetLayers<Overlord>().Where(ov => ov.Alive).ForEach(x => x.Winner = true);
            CallRpc(CustomRPC.WinLose, WinLose.OverlordWins);
        }
    }
}