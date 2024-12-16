namespace TownOfUsReworked.PlayerLayers.Dispositions;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Lovers : Disposition
{
    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool BothLoversDie { get; set; } = true;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool LoversChat { get; set; } = true;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool LoversRoles { get; set; } = true;

    public PlayerControl OtherLover { get; set; }
    public bool LoversAlive => !Player.HasDied() && !OtherLover.HasDied();

    public override UColor Color => ClientOptions.CustomDispColors ? CustomColorManager.Lovers : CustomColorManager.Disposition;
    public override string Name => "Lovers";
    public override string Symbol => "♥";
    public override LayerEnum Type => LayerEnum.Lovers;
    public override Func<string> Description => () => $"- Live to the final 3 with {OtherLover.Data.PlayerName}";

    public override void OnDeath(DeathReason reason, DeathReasonEnum reason2, PlayerControl killer)
    {
        if (Local && BothLoversDie && !OtherLover.HasDied() && !OtherLover.Is(Alignment.NeutralApoc))
            RpcMurderPlayer(OtherLover);
    }
}