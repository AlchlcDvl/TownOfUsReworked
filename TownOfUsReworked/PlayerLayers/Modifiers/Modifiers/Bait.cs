namespace TownOfUsReworked.PlayerLayers.Modifiers;

[LayerHeaderOption(LayerEnum.Bait)]
public sealed class Bait : Modifier
{
    [ToggleOption]
    private static bool BaitKnows = true;

    [NumberOption(0f, 15f, 0.5f, Format.Time)]
    public static Number BaitMinDelay = 0;

    [NumberOption(0f, 15f, 0.5f, Format.Time)]
    public static Number BaitMaxDelay = 1;

    protected override UColor MainColor => CustomColorManager.Bait;
    public override LayerEnum Type => LayerEnum.Bait;
    public override Func<string> Description => () => "- Killing you causes the killer to report your body, albeit with a slight delay";
    public override bool Hidden => !BaitKnows && !Dead;

    public override void OnDeath(DeathReasonEnum reason, PlayerControl killer)
    {
        if (killer != Player)
            BaitReport(killer, Player);
    }
}