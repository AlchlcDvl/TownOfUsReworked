namespace TownOfUsReworked.PlayerLayers.Modifiers;

public class Professional : Modifier
{
    public bool LifeUsed { get; set; }

    public override UColor Color => ClientOptions.CustomModColors ? CustomColorManager.Professional : CustomColorManager.Modifier;
    public override string Name => "Professional";
    public override LayerEnum Type => LayerEnum.Professional;
    public override Func<string> Description => () => "- You have an extra life when assassinating";
    public override bool Hidden => !CustomGameOptions.TraitorKnows && !LifeUsed && !Dead;

    public override void Init() => LifeUsed = false;
}