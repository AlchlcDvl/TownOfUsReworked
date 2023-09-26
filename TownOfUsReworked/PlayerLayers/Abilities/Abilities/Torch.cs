namespace TownOfUsReworked.PlayerLayers.Abilities;

public class Torch : Ability
{
    public override Color Color => ClientGameOptions.CustomAbColors ? Colors.Torch : Colors.Ability;
    public override string Name => "Torch";
    public override LayerEnum Type => LayerEnum.Torch;
    public override Func<string> Description => () => "- You see more than the others";

    public Torch(PlayerControl player) : base(player) {}
}