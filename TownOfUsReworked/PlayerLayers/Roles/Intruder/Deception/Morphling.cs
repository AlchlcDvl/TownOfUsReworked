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

    public Morphling() : base() {}

    public override PlayerLayer Start(PlayerControl player)
    {
        SetPlayer(player);
        BaseStart();
        Alignment = Alignment.IntruderDecep;
        SampledPlayer = null;
        MorphedPlayer = null;
        SampleButton = new(this, "Sample", AbilityTypes.Alive, "Tertiary", Sample, CustomGameOptions.SampleCd, Exception1);
        MorphButton = new(this, "Morph", AbilityTypes.Targetless, "Secondary", HitMorph, CustomGameOptions.MorphCd, CustomGameOptions.MorphDur, (CustomButton.EffectVoid)Morph, UnMorph);
        Data.Role.IntroSound = GetAudio("MorphlingIntro");
        return this;
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
        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction1, MorphButton, MorphedPlayer);
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

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        SampleButton.Update2("SAMPLE");
        MorphButton.Update2("MORPH", SampledPlayer != null);
    }

    public override void TryEndEffect() => MorphButton.Update3(IsDead);

    public override void ReadRPC(MessageReader reader) => MorphedPlayer = reader.ReadPlayer();
}