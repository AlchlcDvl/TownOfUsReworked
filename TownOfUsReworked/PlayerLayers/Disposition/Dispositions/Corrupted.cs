namespace TownOfUsReworked.PlayerLayers.Dispositions;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Corrupted : Disposition
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number CorruptCd = 25;

    [ToggleOption]
    public static bool AllCorruptedWin = false;

    [ToggleOption]
    public static bool CorruptedVent = false;

    private CustomButton CorruptButton { get; set; }

    public override UColor Color => ClientOptions.CustomDispColors ? CustomColorManager.Corrupted : CustomColorManager.Disposition;
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