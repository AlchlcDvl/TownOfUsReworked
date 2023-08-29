namespace TownOfUsReworked.PlayerLayers.Modifiers;

public class Yeller : Modifier
{
    public override Color32 Color => ClientGameOptions.CustomModColors ? Colors.Yeller : Colors.Modifier;
    public override string Name => "Yeller";
    public override LayerEnum Type => LayerEnum.Yeller;
    public override Func<string> Description => () => "- Everyone knows where you are";

    public Yeller(PlayerControl player) : base(player) {}
}