namespace TownOfUsReworked.PlayerLayers.Dispositions;

[LayerHeaderOption(Layer.Corrupted)]
public sealed class Corrupted : Disposition
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number CorruptCd = 25;

    [ToggleOption]
    private static bool CorruptedVent = false;

    private CustomButton CorruptButton;

    protected override UColor MainColor => CustomColorManager.Corrupted;
    public override string Symbol => "δ";
    public override Layer Type => Layer.Corrupted;
    public override string Description => "- Corrupt everyone";
    public override Attack Attack => Attack.Basic;
    public override bool CanVent => CorruptedVent;

    public override void Init()
    {
        CorruptButton ??= new(this, "CORRUPT", new SpriteName("Corrupt"), AbilityTypes.Player, KeybindType.Quarternary, (OnClickPlayer)Corrupt, new Cooldown(CorruptCd));
        Handler.CurrentFaction = Faction.Outcast;
    }

    private void Corrupt(PlayerControl target) => CorruptButton.StartCooldown(Interact(Player, target, true));

    protected override void CheckWin(HashSet<byte> winnerIds)
    {
        if (!GetLayers<Corrupted>().Any(x => x.Alive && x != this))
            return;

        WinState = WinLose.CorruptedWins;
        winnerIds.Add(PlayerId);
    }
}