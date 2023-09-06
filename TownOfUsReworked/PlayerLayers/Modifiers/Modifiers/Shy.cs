namespace TownOfUsReworked.PlayerLayers.Modifiers;

public class Shy : Modifier
{
    public override Color Color => ClientGameOptions.CustomModColors ? Colors.Shy : Colors.Modifier;
    public override string Name => "Shy";
    public override LayerEnum Type => LayerEnum.Shy;
    public override Func<string> Description => () => "- You cannot call meetings";

    public Shy(PlayerControl player) : base(player) {}
}