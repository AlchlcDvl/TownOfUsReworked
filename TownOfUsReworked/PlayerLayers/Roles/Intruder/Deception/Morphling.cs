namespace TownOfUsReworked.PlayerLayers.Roles;

public class Morphling : Intruder
{
    public CustomButton MorphButton { get; set; }
    public CustomButton SampleButton { get; set; }
    public PlayerControl MorphedPlayer { get; set; }
    public PlayerControl SampledPlayer { get; set; }
    public bool Morphed => MorphButton.EffectActive;

    public override UColor Color => ClientGameOptions.CustomIntColors ? CustomColorManager.Morphling : CustomColorManager.Intruder;
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
        SampleButton = CreateButton(this, new SpriteName("Sample"), AbilityTypes.Alive, KeybindType.Tertiary, (OnClick)Sample, new Cooldown (CustomGameOptions.SampleCd), "SAMPLE",
            (PlayerBodyExclusion)Exception1);
        MorphButton = CreateButton(this, new SpriteName("Morph"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClick)HitMorph, new Cooldown(CustomGameOptions.MorphCd), "MORPH",
            new Duration(CustomGameOptions.MorphDur), (EffectVoid)Morph, (EffectEndVoid)UnMorph, (EndFunc)EndEffect, (UsableFunc)Usable);
        Data.Role.IntroSound = GetAudio("MorphlingIntro");
    }

    public void Morph() => Utils.Morph(Player, MorphedPlayer);

    public void UnMorph()
    {
        MorphedPlayer = null;
        DefaultOutfit(Player);

        if (CustomGameOptions.MorphCooldownsLinked)
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

        if (CustomGameOptions.DisgCooldownsLinked)
            MorphButton.StartCooldown(cooldown);
    }

    public bool Exception1(PlayerControl player) => player == SampledPlayer;

    public bool Usable() => SampledPlayer;

    public bool EndEffect() => Dead;

    public override void ReadRPC(MessageReader reader) => MorphedPlayer = reader.ReadPlayer();
}