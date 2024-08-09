namespace TownOfUsReworked.PlayerLayers.Modifiers;

public class Diseased : Modifier
{
    public override UColor Color => ClientOptions.CustomModColors ? CustomColorManager.Diseased : CustomColorManager.Modifier;
    public override string Name => "Diseased";
    public override LayerEnum Type => LayerEnum.Diseased;
    public override Func<string> Description => () => $"- Your killer's cooldown increases by {CustomGameOptions.DiseasedMultiplier} times";
    public override bool Hidden => !CustomGameOptions.DiseasedKnows && !Dead;
}