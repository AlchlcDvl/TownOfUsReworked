namespace TownOfUsReworked.PlayerLayers.Dispositions;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Corrupted : Disposition
{
    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number CorruptCd { get; set; } = new(25);

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool AllCorruptedWin { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool CorruptedVent { get; set; } = false;

    private CustomButton CorruptButton { get; set; }

    public override UColor Color => ClientOptions.CustomDispColors ? CustomColorManager.Corrupted : CustomColorManager.Disposition;
    public override string Name => "Corrupted";
    public override string Symbol => "δ";
    public override LayerEnum Type => LayerEnum.Corrupted;
    public override Func<string> Description => () => "- Corrupt everyone";
    public override AttackEnum AttackVal => AttackEnum.Basic;

    public override void Init()
    {
        base.Init();
        CorruptButton ??= new(this, "CORRUPT", new SpriteName("Corrupt"), AbilityTypes.Player, KeybindType.Quarternary, (OnClickPlayer)Corrupt, new Cooldown(CorruptCd));
        Player.GetRole().Faction = Faction.Neutral;
    }

    private void Corrupt(PlayerControl target) => CorruptButton.StartCooldown(Interact(Player, target, true));

    public override void CheckWin()
    {
        if (CorruptedWin(Player))
        {
            WinState = WinLose.CorruptedWins;

            if (AllCorruptedWin)
                GetLayers<Corrupted>().ForEach(x => x.Winner = true);
            else
                Winner = true;

            CallRpc(CustomRPC.WinLose, WinLose.CorruptedWins, this);
        }
    }
}