namespace TownOfUsReworked.PlayerLayers.Abilities;

public class Multitasker : Ability
{
    public override UColor Color => ClientGameOptions.CustomAbColors ? CustomColorManager.Multitasker : CustomColorManager.Ability;
    public override string Name => "Multitasker";
    public override LayerEnum Type => LayerEnum.Multitasker;
    public override Func<string> Description => () => "- Your task windows are transparent";
}