namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Morphling)]
public sealed class Morphling : Deception
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number MorphCd = 25;

    [NumberOption(5f, 30f, 1f, Format.Time)]
    private static Number MorphDur = 10;

    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number SampleCd = 25;

    [ToggleOption]
    private static bool MorphCooldownsLinked = false;

    [ToggleOption]
    private static bool MorphlingVent = false;

    private CustomButton MorphButton;
    private CustomButton SampleButton;
    private PlayerControl SampledPlayer;
    private bool ClickedAgain;

    protected override UColor MainColor => CustomColorManager.Morphling;
    public override LayerEnum Type => LayerEnum.Morphling;
    public override string StartText => "Fool The <#8CFFFFFF>Crew</color> With Your Appearances";
    public override string Description => $"- You can morph into other players, taking up their appearances as your own\n{CommonAbilities}";
    public override bool CanVent => MorphlingVent;

    public override void Init()
    {
        base.Init();
        SampledPlayer = null;
        SampleButton ??= new(this, new SpriteName("Sample"), AbilityTypes.Player, KeybindType.Tertiary, (OnClickPlayer)Sample, new Cooldown (SampleCd), "SAMPLE", (PlayerBodyExclusion)Exception1);
        MorphButton ??= new(this, new SpriteName("Morph"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClickTargetless)HitMorph, new Cooldown(MorphCd), "MORPH", (EffectEndVoid)UnMorph,
            new Duration(MorphDur), (EndFunc)EndEffect, (UsableFunc)Usable, (ClickedAgainVoid)OnClickedAgain, (EffectStartVoid)Morph);
    }

    public override void Reset(bool meeting, bool start) => SampledPlayer = null;

    private void Morph() => Player.SetMimicked(SampledPlayer, MorphDur, EndEffect);

    private void UnMorph()
    {
        ClickedAgain = false;

        if (MorphCooldownsLinked)
            SampleButton.StartCooldown();
    }

    private void HitMorph()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, MorphButton, SampledPlayer);
        MorphButton.Begin();
        PopNotif("<b>You have morphed into " + SampledPlayer.name + "!</b>", Color);
    }

    private void Sample(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
            SampledPlayer = target;

        SampleButton.StartCooldown(cooldown);

        if (MorphCooldownsLinked)
            MorphButton.StartCooldown(cooldown);
    }

    private bool Exception1(PlayerControl player) => player == SampledPlayer;

    private bool Usable() => SampledPlayer;

    private bool EndEffect() => Dead || ClickedAgain;

    public override void ReadRPC(RpcReader reader) => SampledPlayer = reader.ReadPlayer();

    private void OnClickedAgain() => ClickedAgain = true;
}