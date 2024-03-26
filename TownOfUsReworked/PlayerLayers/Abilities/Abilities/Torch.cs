namespace TownOfUsReworked.PlayerLayers.Abilities;

public class Torch : Ability
{
    public override UColor Color => ClientGameOptions.CustomAbColors ? CustomColorManager.Torch : CustomColorManager.Ability;
    public override string Name => "Torch";
    public override LayerEnum Type => LayerEnum.Torch;
    public override Func<string> Description => () => "- You see more than the others";
}