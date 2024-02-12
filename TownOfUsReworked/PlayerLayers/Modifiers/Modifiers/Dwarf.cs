namespace TownOfUsReworked.PlayerLayers.Modifiers;

public class Dwarf : Modifier
{
    private static bool Smol => CustomGameOptions.DwarfScale != 1;
    private static bool Sped => CustomGameOptions.DwarfSpeed != 1;
    private static bool Useless => !Smol && !Sped;
    private static string Text => Smol && Sped ? "tiny and speedy" : (Smol ? "tiny" : (Sped ? "speedy" : ""));

    public override UColor Color => ClientGameOptions.CustomModColors ? (Useless ? CustomColorManager.Modifier : CustomColorManager.Dwarf) : CustomColorManager.Modifier;
    public override string Name => Useless ? "Useless" : (!Smol ? "Flash" : (Sped ? "Gremlin" : "Dwarf"));
    public override LayerEnum Type => LayerEnum.Dwarf;
    public override Func<string> Description => () => Useless ? "- Why" : $"- You are {Text}";

    public Dwarf() : base() {}

    public override PlayerLayer Start(PlayerControl player)
    {
        SetPlayer(player);
        return this;
    }
}