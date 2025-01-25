namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Mayor : Crew, IRevealer
{
    [NumberOption(1, 10, 1)]
    public static Number MayorVoteCount = 2;

    [ToggleOption]
    public static bool RoundOneNoMayorReveal = false;

    [ToggleOption]
    public static bool MayorButton = true;

    public bool RoundOne { get; set; }
    public CustomButton RevealButton { get; set; }
    public bool Revealed { get; set; }

    public override UColor Color => ClientOptions.CustomCrewColors ? CustomColorManager.Mayor : FactionColor;
    public override LayerEnum Type => LayerEnum.Mayor;
    public override Func<string> StartText => () => "Reveal Yourself To Commit Voter Fraud";
    public override Func<string> Description => () => $"- You can reveal yourself to the crew\n- When revealed, your votes count {MayorVoteCount + 1} times but you cannot be protected";

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.Sovereign;
        RevealButton ??= new(this, new SpriteName("MayorReveal"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClickTargetless)Reveal, (UsableFunc)Usable);
    }

    public void Reveal()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.PublicReveal, Player);
        PublicReveal(Player);
    }

    public bool Usable() => !RoundOne && !Revealed && !GetLayers<Mayor>().Any(x => !x.TrulyDead && x.Revealed);

    // public override void UpdateSelfName(ref string name, ref UColor color, ref bool revealed, ref bool removeFromConsig)
    // {
    //     if (Revealed)
    //     {
    //         revealed = true;
    //         name += $"\n{Name}";
    //         color = Color;
    //         removeFromConsig = true;
    //     }
    // }
}