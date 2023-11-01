namespace TownOfUsReworked.PlayerLayers.Roles;

public class Morphling : Intruder
{
    public CustomButton MorphButton { get; set; }
    public CustomButton SampleButton { get; set; }
    public PlayerControl MorphedPlayer { get; set; }
    public PlayerControl SampledPlayer { get; set; }
    public bool Morphed => MorphButton.EffectActive;

    public override Color Color => ClientGameOptions.CustomIntColors ? Colors.Morphling : Colors.Intruder;
    public override string Name => "Morphling";
    public override LayerEnum Type => LayerEnum.Morphling;
    public override Func<string> StartText => () => "Fool The <color=#8CFFFFFF>Crew</color> With Your Appearances";
    public override Func<string> Description => () => $"- You can morph into other players, taking up their appearances as your own\n{CommonAbilities}";

    public Morphling(PlayerControl player) : base(player)
    {
        Alignment = Alignment.IntruderDecep;
        SampledPlayer = null;
        MorphedPlayer = null;
        MorphButton = new(this, "Morph", AbilityTypes.Targetless, "Secondary", HitMorph, CustomGameOptions.MorphCd, CustomGameOptions.MorphDur, (CustomButton.EffectVoid)Morph, UnMorph);
        SampleButton = new(this, "Sample", AbilityTypes.Target, "Tertiary", Sample, CustomGameOptions.SampleCd, Exception1);
        player.Data.Role.IntroSound = GetAudio("MorphlingIntro");
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
        var interact = Interact(Player, SampleButton.TargetPlayer);
        var cooldown = CooldownType.Reset;

        if (interact.AbilityUsed)
            SampledPlayer = SampleButton.TargetPlayer;

        if (interact.Protected)
            cooldown = CooldownType.GuardianAngel;

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