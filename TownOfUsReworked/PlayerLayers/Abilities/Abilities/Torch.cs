namespace TownOfUsReworked.PlayerLayers.Abilities;

public class Torch : Ability
{
    public override UColor Color => ClientOptions.CustomAbColors ? CustomColorManager.Torch : CustomColorManager.Ability;
    public override LayerEnum Type => LayerEnum.Torch;
    public override Func<string> Description => () => "- You see more than the others";
}