namespace TownOfUsReworked.PlayerLayers.Modifiers;

[LayerHeaderOption(Layer.Diseased)]
public sealed class Diseased : Modifier
{
    [ToggleOption]
    private static bool DiseasedKnows = true;

    [NumberOption(1.5f, 5f, 0.5f, Format.Multiplier)]
    public static Number DiseasedMultiplier = 3;

    protected override UColor MainColor => CustomColorManager.Diseased;
    public override Layer Type => Layer.Diseased;
    public override string Description => $"- Your killer's cooldown increases by {DiseasedMultiplier} times";
    public override bool Hidden => !DiseasedKnows && !Dead;

    public override void OnDeath(DeathReasonEnum reason, PlayerControl killer)
    {
        if (killer != Player && LayerHandler.Handlers.TryGetValue(killer.PlayerId, out var handler))
            handler.Diseased = true;
    }
}