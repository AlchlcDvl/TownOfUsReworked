namespace TownOfUsReworked.PlayerLayers.Modifiers;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Diseased : Modifier
{
    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool DiseasedKnows { get; set; } = true;

    [NumberOption(MultiMenu.LayerSubOptions, 1.5f, 5f, 0.5f, Format.Multiplier)]
    public static Number DiseasedMultiplier { get; set; } = new(3);

    public override UColor Color => ClientOptions.CustomModColors ? CustomColorManager.Diseased : CustomColorManager.Modifier;
    public override string Name => "Diseased";
    public override LayerEnum Type => LayerEnum.Diseased;
    public override Func<string> Description => () => $"- Your killer's cooldown increases by {DiseasedMultiplier} times";
    public override bool Hidden => !DiseasedKnows && !Dead;

    public override void OnDeath(DeathReason reason, DeathReasonEnum reason2, PlayerControl killer)
    {
        if (killer != Player)
            killer.GetRole().Diseased = true;
    }
}