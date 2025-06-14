namespace TownOfUsReworked.PlayerLayers.Dispositions;

[LayerHeaderOption(LayerEnum.Lovers)]
public sealed class Lovers : Paired
{
    [ToggleOption]
    public static bool BothLoversDie = true;

    [ToggleOption]
    public static bool LoversChat = true;

    [ToggleOption]
    private static bool LoversRoles = true;

    [ToggleOption]
    public static bool ConvertLovers = true;

    private bool LoversAlive => !Player.HasDied() && !Other.HasDied();

    protected override UColor MainColor => CustomColorManager.Lovers;
    public override string Symbol => "♥";
    public override LayerEnum Type => LayerEnum.Lovers;
    public override string Description => $"- Live to the final 3 with {Other.name}";
    protected override bool RevealRole => LoversRoles;
    protected override ChatChannel Channel => ChatChannel.Lovers;

    public override void OnDeath(DeathReasonEnum reason, PlayerControl killer)
    {
        if (BothLoversDie && !Other.HasDied() && !Other.Is(Alignment.Deity))
            Other.Suicide();
    }

    protected override void CheckWin(HashSet<byte> winnerIds)
    {
        if (AllPlayers().Count(x => !x.HasDied()) <= 3 && LoversAlive)
            return;

        WinState = WinLose.LoveWins;
        winnerIds.Add(PlayerId);
    }
}