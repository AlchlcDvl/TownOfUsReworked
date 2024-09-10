namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Mayor : Crew
{
    [NumberOption(MultiMenu.LayerSubOptions, 1, 10, 1)]
    public static Number MayorVoteCount { get; set; } = new(2);

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool RoundOneNoMayorReveal { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool MayorButton { get; set; } = true;

    public bool RoundOne { get; set; }
    public CustomButton RevealButton { get; set; }
    public bool Revealed { get; set; }

    public override UColor Color => ClientOptions.CustomCrewColors ? CustomColorManager.Mayor : CustomColorManager.Crew;
    public override string Name => "Mayor";
    public override LayerEnum Type => LayerEnum.Mayor;
    public override Func<string> StartText => () => "Reveal Yourself To Commit Voter Fraud";
    public override Func<string> Description => () => $"- You can reveal yourself to the crew\n- When revealed, your votes count {MayorVoteCount + 1} times but you cannot be protected";

    public override void Init()
    {
        BaseStart();
        Alignment = Alignment.CrewSov;
        RevealButton = CreateButton(this, new SpriteName("MayorReveal"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClick)Reveal);
        Data.Role.IntroSound = GetAudio("MayorIntro");
    }

    public void Reveal()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.PublicReveal, Player);
        PublicReveal(Player);
    }
}