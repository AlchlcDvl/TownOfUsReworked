namespace TownOfUsReworked.PlayerLayers.Dispositions;

[LayerHeaderOption(LayerEnum.Rivals)]
public sealed class Rivals : Paired
{
    [ToggleOption]
    public static bool RivalsChat = true;

    [ToggleOption]
    public static bool RivalsRoles = true;

    public bool IsWinningRival => Other.HasDied() && !Player.HasDied();

    protected override UColor MainColor => CustomColorManager.Rivals;
    public override string Symbol => "α";
    public override LayerEnum Type => LayerEnum.Rivals;
    public override Func<string> Description => () => Other.HasDied() ? "- Live to the final 2" : $"- Get {Other.name} killed";
    protected override bool RevealRole => RivalsRoles;
    protected override ChatChannel Channel => ChatChannel.Rivals;

    protected override void CheckWin(HashSet<byte> winnerIds)
    {
        if (AllPlayers().Count(x => !x.HasDied()) <= 2 &&  IsWinningRival)
            return;

        WinState = WinLose.RivalWins;
        winnerIds.Add(PlayerId);
    }
}