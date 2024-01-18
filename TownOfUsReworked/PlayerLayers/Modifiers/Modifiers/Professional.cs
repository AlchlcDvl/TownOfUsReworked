namespace TownOfUsReworked.PlayerLayers.Modifiers;

public class Professional : Modifier
{
    public bool LifeUsed { get; set; }

    public override UColor Color => ClientGameOptions.CustomModColors ? CustomColorManager.Professional : CustomColorManager.Modifier;
    public override string Name => "Professional";
    public override LayerEnum Type => LayerEnum.Professional;
    public override Func<string> Description => () => "- You have an extra life when assassinating";
    public override bool Hidden => !CustomGameOptions.TraitorKnows && !LifeUsed && !IsDead;

    public Professional(PlayerControl player) : base(player) => LifeUsed = false;
}