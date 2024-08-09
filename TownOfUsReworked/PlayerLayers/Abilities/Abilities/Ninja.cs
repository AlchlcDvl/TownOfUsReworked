namespace TownOfUsReworked.PlayerLayers.Abilities;

public class Ninja : Ability
{
    public override UColor Color => ClientOptions.CustomAbColors ? CustomColorManager.Ninja : CustomColorManager.Ability;
    public override string Name => "Ninja";
    public override LayerEnum Type => LayerEnum.Ninja;
    public override Func<string> Description => () => "- You do not lunge when killing";
}