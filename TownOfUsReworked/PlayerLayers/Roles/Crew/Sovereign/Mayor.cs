namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Mayor)]
public sealed class Mayor : Sovereign
{
    [NumberOption(1, 10, 1)]
    public static Number MayorVoteCount = 2;

    [ToggleOption]
    public static bool MayorButton = true;

    [ToggleOption]
    public static bool MayorDirectSpawn = true;

    [ToggleOption]
    private static bool RoundOneNoMayorReveal = true;

    private bool RoundOne { get; set; }
    private CustomButton RevealButton { get; set; }

    protected override UColor MainColor => CustomColorManager.Mayor;
    public override LayerEnum Type => LayerEnum.Mayor;
    public override Func<string> StartText { get; } = () => "Commit Voter Fraud!";
    public override Func<string> Description => () => $"-Your votes count {MayorVoteCount + 1} times but you cannot be protected";

    protected override void Init()
    {
        base.Init();
        RevealButton ??= new(this, new SpriteName("MayorReveal"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClickTargetless)Reveal, (UsableFunc)Usable, "REVEAL");
    }

    public override void Reset(bool meeting, bool start) => RoundOne = start && RoundOneNoMayorReveal;

    private void Reveal()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.PublicReveal, Player);
        PublicReveal(Player);

        if (!MayorDirectSpawn && MeetingPatches.MeetingCount <= 3)
            CustomAchievementManager.UnlockAchievement("Persuasive");
    }

    private bool Usable() => !RoundOne && !GetLayers<Mayor>().Any(x => !x.TrulyDead && x.Revealed);

    public override void UpdateSelfName(ref string name, ref UColor color, ref bool revealed, ref bool removeFromConsig)
    {
        revealed = true;
        name += $"\n{Name}";
        color = Color;
        removeFromConsig = true;
    }
}