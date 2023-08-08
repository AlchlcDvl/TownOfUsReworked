namespace TownOfUsReworked.PlayerLayers.Roles;

public class Morphling : Intruder
{
    public CustomButton MorphButton { get; set; }
    public CustomButton SampleButton { get; set; }
    public DateTime LastMorphed { get; set; }
    public DateTime LastSampled { get; set; }
    public PlayerControl MorphedPlayer { get; set; }
    public PlayerControl SampledPlayer { get; set; }
    public float TimeRemaining { get; set; }
    public bool Enabled { get; set; }
    public bool Morphed => TimeRemaining > 0f;

    public override Color32 Color => ClientGameOptions.CustomIntColors ? Colors.Morphling : Colors.Intruder;
    public override string Name => "Morphling";
    public override LayerEnum Type => LayerEnum.Morphling;
    public override Func<string> StartText => () => "Fool The <color=#8CFFFFFF>Crew</color> With Your Appearances";
    public override Func<string> Description => () => $"- You can morph into other players, taking up their appearances as your own\n{CommonAbilities}";
    public override InspectorResults InspectorResults => InspectorResults.CreatesConfusion;
    public float SampleTimer => ButtonUtils.Timer(Player, LastSampled, CustomGameOptions.SampleCooldown);
    public float MorphTimer => ButtonUtils.Timer(Player, LastMorphed, CustomGameOptions.MorphlingCd);

    public Morphling(PlayerControl player) : base(player)
    {
        RoleAlignment = RoleAlignment.IntruderDecep;
        SampledPlayer = null;
        MorphedPlayer = null;
        MorphButton = new(this, "Morph", AbilityTypes.Effect, "Secondary", HitMorph);
        SampleButton = new(this, "Sample", AbilityTypes.Direct, "Tertiary", Sample, Exception1);
    }

    public void Morph()
    {
        TimeRemaining -= Time.deltaTime;
        Utils.Morph(Player, MorphedPlayer);
        Enabled = true;

        if (IsDead || Meeting)
            TimeRemaining = 0f;
    }

    public void Unmorph()
    {
        MorphedPlayer = null;
        Enabled = false;
        DefaultOutfit(Player);
        LastMorphed = DateTime.UtcNow;

        if (CustomGameOptions.MorphCooldownsLinked)
            LastSampled = DateTime.UtcNow;
    }

    public void HitMorph()
    {
        if (MorphTimer != 0f || SampledPlayer == null || Morphed)
            return;

        TimeRemaining = CustomGameOptions.MorphlingDuration;
        MorphedPlayer = SampledPlayer;
        CallRpc(CustomRPC.Action, ActionsRPC.Morph, this, MorphedPlayer);
        Morph();
    }

    public void Sample()
    {
        if (SampleTimer != 0f || IsTooFar(Player, SampleButton.TargetPlayer) || SampledPlayer == SampleButton.TargetPlayer)
            return;

        var interact = Interact(Player, SampleButton.TargetPlayer);

        if (interact[3])
            SampledPlayer = SampleButton.TargetPlayer;

        if (interact[0])
        {
            LastSampled = DateTime.UtcNow;

            if (CustomGameOptions.MorphCooldownsLinked)
                LastMorphed = DateTime.UtcNow;
        }
        else if (interact[1])
        {
            LastSampled.AddSeconds(CustomGameOptions.ProtectKCReset);

            if (CustomGameOptions.MorphCooldownsLinked)
                LastMorphed.AddSeconds(CustomGameOptions.ProtectKCReset);
        }
    }

    public bool Exception1(PlayerControl player) => player == SampledPlayer;

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        MorphButton.Update("MORPH", MorphTimer, CustomGameOptions.MorphlingCd, Morphed, TimeRemaining, CustomGameOptions.MorphlingDuration, true, SampledPlayer != null);
        SampleButton.Update("SAMPLE", SampleTimer, CustomGameOptions.MeasureCooldown);
    }
}