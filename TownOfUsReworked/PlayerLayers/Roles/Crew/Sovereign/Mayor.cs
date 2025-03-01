namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Mayor : Crew, IRevealer
{
    [NumberOption(1, 10, 1)]
    public static Number MayorVoteCount = 2;

    [ToggleOption]
    public static bool MayorButton = true;

    // This is cursed
    public bool Revealed
    {
        get => true;
        set {}
    }

    public override UColor Color => ClientOptions.CustomCrewColors ? CustomColorManager.Mayor : FactionColor;
    public override LayerEnum Type => LayerEnum.Mayor;
    public override Func<string> StartText => () => "Commit Voter Fraud!";
    public override Func<string> Description => () => $"-Your votes count {MayorVoteCount + 1} times but you cannot be protected";

    protected override void Init()
    {
        base.Init();
        Alignment = Alignment.Sovereign;
    }

    public override void UpdateSelfName(ref string name, ref UColor color, ref bool revealed, ref bool removeFromConsig)
    {
        revealed = true;
        name += $"\n{Name}";
        color = Color;
        removeFromConsig = true;
    }
}