namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Morphling : Intruder
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number MorphCd = 25;

    [NumberOption(5f, 30f, 1f, Format.Time)]
    public static Number MorphDur = 10;

    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number SampleCd = 25;

    [ToggleOption]
    public static bool MorphCooldownsLinked = false;

    [ToggleOption]
    public static bool MorphlingVent = false;

    public CustomButton MorphButton { get; set; }
    public CustomButton SampleButton { get; set; }
    public PlayerControl MorphedPlayer { get; set; }
    public PlayerControl SampledPlayer { get; set; }
    public bool Morphed => MorphButton.EffectActive;

    public override UColor Color => ClientOptions.CustomIntColors ? CustomColorManager.Morphling : FactionColor;
    public override LayerEnum Type => LayerEnum.Morphling;
    public override Func<string> StartText => () => "Fool The <#8CFFFFFF>Crew</color> With Your Appearances";
    public override Func<string> Description => () => $"- You can morph into other players, taking up their appearances as your own\n{CommonAbilities}";

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.Deception;
        SampledPlayer = null;
        MorphedPlayer = null;
        SampleButton ??= new(this, new SpriteName("Sample"), AbilityTypes.Player, KeybindType.Tertiary, (OnClickPlayer)Sample, new Cooldown (SampleCd), "SAMPLE", (PlayerBodyExclusion)Exception1);
        MorphButton ??= new(this, new SpriteName("Morph"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClickTargetless)HitMorph, new Cooldown(MorphCd), "MORPH", (EffectEndVoid)UnMorph,
            new Duration(MorphDur), (EffectVoid)Morph, (EndFunc)EndEffect, (UsableFunc)Usable);
    }

    public void Morph() => Utils.Morph(Player, MorphedPlayer);

    public void UnMorph()
    {
        MorphedPlayer = null;
        DefaultOutfit(Player);

        if (MorphCooldownsLinked)
            SampleButton.StartCooldown();
    }

    public void HitMorph()
    {
        MorphedPlayer = SampledPlayer;
        CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, MorphButton, MorphedPlayer);
        MorphButton.Begin();
    }

    public void Sample(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
            SampledPlayer = target;

        SampleButton.StartCooldown(cooldown);

        if (MorphCooldownsLinked)
            MorphButton.StartCooldown(cooldown);
    }

    public bool Exception1(PlayerControl player) => player == SampledPlayer;

    public bool Usable() => SampledPlayer;

    public bool EndEffect() => Dead;

    public override void ReadRPC(MessageReader reader) => MorphedPlayer = reader.ReadPlayer();
}