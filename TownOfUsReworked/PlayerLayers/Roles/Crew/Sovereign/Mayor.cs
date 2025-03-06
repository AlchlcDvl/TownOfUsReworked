namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public sealed class Mayor : Crew, IRevealer
{
    [NumberOption(1, 10, 1)]
    public static Number MayorVoteCount = 2;

    [ToggleOption]
    public static bool MayorButton = true;

    [ToggleOption]
    public static bool MayorDirectSpawn = true;

    [ToggleOption]
    public static bool RoundOneNoReveal = true;

    public bool Revealed { get; set; }
    public bool RoundOne { get; set; }
    private CustomButton RevealButton { get; set; }

    public override UColor Color => ClientOptions.CustomCrewColors ? CustomColorManager.Mayor : FactionColor;
    public override LayerEnum Type => LayerEnum.Mayor;
    public override Func<string> StartText => () => "Commit Voter Fraud!";
    public override Func<string> Description => () => $"-Your votes count {MayorVoteCount + 1} times but you cannot be protected";

    protected override void Init()
    {
        base.Init();
        Revealed = !MayorDirectSpawn;
        Alignment = Alignment.Sovereign;

        if (!Revealed)
            RevealButton ??= new(this, new SpriteName("MayorReveal"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClickTargetless)Reveal, (UsableFunc)Usable, "REVEAL");
    }

    public override void Reset(bool meeting, bool start) => RoundOne = start && RoundOneNoReveal;

    private void Reveal()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.PublicReveal, Player);
        PublicReveal(Player);
    }

    private bool Usable() => !RoundOne && !GetLayers<Mayor>().Any(x => !x.TrulyDead && x.Revealed);

    public override void UpdateSelfName(ref string name, ref UColor color, ref bool revealed, ref bool removeFromConsig)
    {
        revealed = true;
        name += $"\n{Name}";
        color = Color;
        removeFromConsig = true;
    }

    public void OnReveal() {}
}