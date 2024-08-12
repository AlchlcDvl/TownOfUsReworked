namespace TownOfUsReworked.PlayerLayers.Modifiers;

[HeaderOption(MultiMenu2.LayerSubOptions)]
public class Bait : Modifier
{
    public override UColor Color => ClientOptions.CustomModColors ? CustomColorManager.Bait : CustomColorManager.Modifier;
    public override string Name => "Bait";
    public override LayerEnum Type => LayerEnum.Bait;
    public override Func<string> Description => () => "- Killing you causes the killer to report your body, albeit with a slight delay";
    public override bool Hidden => !CustomGameOptions.BaitKnows && !Dead;
}