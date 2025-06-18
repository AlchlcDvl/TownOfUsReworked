namespace TownOfUsReworked.PlayerLayers.Dispositions;

[LayerHeaderOption(Layer.Rivals)]
public sealed class Rivals : Paired
{
    [ToggleOption]
    public static bool RivalsChat = true;

    [ToggleOption]
    private static bool RivalsRoles = true;

    private bool IsWinningRival => Other.HasDied() && !Player.HasDied();

    protected override UColor MainColor => CustomColorManager.Rivals;
    public override string Symbol => "α";
    public override Layer Type => Layer.Rivals;
    public override string Description => Other.HasDied() ? "- Live to the final 2" : $"- Get {Other.name} killed";
    protected override bool RevealRole => RivalsRoles;
    public override ChatChannel Channel => ChatChannel.Rivals;
    public override bool CanChat => RivalsChat;

    protected override void CheckWin(HashSet<byte> winnerIds)
    {
        if (AllPlayers().Count(x => !x.HasDied()) <= 2 && IsWinningRival)
            return;

        WinState = WinLose.RivalWins;
        winnerIds.Add(PlayerId);
    }
}