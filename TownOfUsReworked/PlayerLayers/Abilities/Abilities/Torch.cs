namespace TownOfUsReworked.PlayerLayers.Abilities;

public class Torch : Ability
{
    public override Color Color => ClientGameOptions.CustomAbColors ? Colors.Torch : Colors.Ability;
    public override string Name => "Torch";
    public override LayerEnum Type => LayerEnum.Torch;
    public override Func<string> Description => () => "- You can see in the dark";

    public Torch(PlayerControl player) : base(player) {}
}