namespace TownOfUsReworked.PlayerLayers.Modifiers;

[LayerHeaderOption(LayerEnum.Diseased)]
public sealed class Diseased : Modifier
{
    [ToggleOption]
    private static bool DiseasedKnows = true;

    [NumberOption(1.5f, 5f, 0.5f, Format.Multiplier)]
    public static Number DiseasedMultiplier = 3;

    public override UColor Color => ClientOptions.CustomModColors ? CustomColorManager.Diseased : CustomColorManager.Modifier;
    public override LayerEnum Type => LayerEnum.Diseased;
    public override Func<string> Description => () => $"- Your killer's cooldown increases by {DiseasedMultiplier} times";
    public override bool Hidden => !DiseasedKnows && !Dead;

    public override void OnDeath(DeathReason reason, DeathReasonEnum reason2, PlayerControl killer)
    {
        if (killer != Player)
            killer.GetRole().Diseased = true;
    }
}