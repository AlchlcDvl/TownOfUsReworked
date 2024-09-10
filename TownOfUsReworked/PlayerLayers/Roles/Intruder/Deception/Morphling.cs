namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Morphling : Intruder
{
    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number MorphCd { get; set; } = new(25);

    [NumberOption(MultiMenu.LayerSubOptions, 5f, 30f, 1f, Format.Time)]
    public static Number MorphDur { get; set; } = new(10);

    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number SampleCd { get; set; } = new(25);

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool MorphCooldownsLinked { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool MorphlingVent { get; set; } = false;

    public CustomButton MorphButton { get; set; }
    public CustomButton SampleButton { get; set; }
    public PlayerControl MorphedPlayer { get; set; }
    public PlayerControl SampledPlayer { get; set; }
    public bool Morphed => MorphButton.EffectActive;

    public override UColor Color => ClientOptions.CustomIntColors ? CustomColorManager.Morphling : CustomColorManager.Intruder;
    public override string Name => "Morphling";
    public override LayerEnum Type => LayerEnum.Morphling;
    public override Func<string> StartText => () => "Fool The <color=#8CFFFFFF>Crew</color> With Your Appearances";
    public override Func<string> Description => () => $"- You can morph into other players, taking up their appearances as your own\n{CommonAbilities}";

    public override void Init()
    {
        BaseStart();
        Alignment = Alignment.IntruderDecep;
        SampledPlayer = null;
        MorphedPlayer = null;
        SampleButton = CreateButton(this, new SpriteName("Sample"), AbilityTypes.Alive, KeybindType.Tertiary, (OnClick)Sample, new Cooldown (SampleCd), "SAMPLE",
            (PlayerBodyExclusion)Exception1);
        MorphButton = CreateButton(this, new SpriteName("Morph"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClick)HitMorph, new Cooldown(MorphCd), "MORPH", (EffectEndVoid)UnMorph,
            new Duration(MorphDur), (EffectVoid)Morph, (EndFunc)EndEffect, (UsableFunc)Usable);
        Data.Role.IntroSound = GetAudio("MorphlingIntro");
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

    public void Sample()
    {
        var cooldown = Interact(Player, SampleButton.TargetPlayer);

        if (cooldown != CooldownType.Fail)
            SampledPlayer = SampleButton.TargetPlayer;

        SampleButton.StartCooldown(cooldown);

        if (MorphCooldownsLinked)
            MorphButton.StartCooldown(cooldown);
    }

    public bool Exception1(PlayerControl player) => player == SampledPlayer;

    public bool Usable() => SampledPlayer;

    public bool EndEffect() => Dead;

    public override void ReadRPC(MessageReader reader) => MorphedPlayer = reader.ReadPlayer();
}