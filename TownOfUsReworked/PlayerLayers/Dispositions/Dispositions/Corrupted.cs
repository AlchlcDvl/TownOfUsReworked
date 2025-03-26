namespace TownOfUsReworked.PlayerLayers.Dispositions;

[LayerHeaderOption(LayerEnum.Corrupted)]
public sealed class Corrupted : Disposition
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number CorruptCd = 25;

    [ToggleOption]
    public static bool AllCorruptedWin = false;

    [ToggleOption]
    private static bool CorruptedVent = false;

    private CustomButton CorruptButton { get; set; }

    protected override UColor MainColor => CustomColorManager.Corrupted;
    public override string Symbol => "δ";
    public override LayerEnum Type => LayerEnum.Corrupted;
    public override Func<string> Description => () => "- Corrupt everyone";
    public override AttackEnum AttackVal => AttackEnum.Basic;
    public override bool CanVent => CorruptedVent;

    protected override void Init()
    {
        base.Init();
        CorruptButton ??= new(this, "CORRUPT", new SpriteName("Corrupt"), AbilityTypes.Player, KeybindType.Quarternary, (OnClickPlayer)Corrupt, new Cooldown(CorruptCd));
        Player.GetRole().Faction = Faction.Neutral;
    }

    private void Corrupt(PlayerControl target) => CorruptButton.StartCooldown(Interact(Player, target, true));

    protected override void CheckWin()
    {
        if (!CorruptedWin(Player))
            return;

        WinState = WinLose.CorruptedWins;

        if (AllCorruptedWin)
            GetLayers<Corrupted>().ForEach(x => x.Winner = true);
        else
            Winner = true;

        CallRpc(CustomRPC.WinLose, WinLose.CorruptedWins, this);
    }
}