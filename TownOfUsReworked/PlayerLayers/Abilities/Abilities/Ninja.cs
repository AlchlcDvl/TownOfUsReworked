namespace TownOfUsReworked.PlayerLayers.Abilities;

public class Ninja : Ability
{
    public override Color Color => ClientGameOptions.CustomAbColors ? Colors.Ninja : Colors.Ability;
    public override string Name => "Ninja";
    public override LayerEnum Type => LayerEnum.Ninja;
    public override Func<string> Description => () => "- You do not lunge when killing";

    public Ninja(PlayerControl player) : base(player) {}
}