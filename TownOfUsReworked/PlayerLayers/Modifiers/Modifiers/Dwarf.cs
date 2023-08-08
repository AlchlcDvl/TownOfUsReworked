namespace TownOfUsReworked.PlayerLayers.Modifiers;

public class Dwarf : Modifier
{
    private static bool Smol => CustomGameOptions.DwarfScale != 1;
    private static bool Sped => CustomGameOptions.DwarfSpeed != 1;
    private static string Text => Smol && Sped ? "tiny and speedy" : (Smol ? "tiny" : (Sped ? "speedy" : ""));

    public override Color32 Color => ClientGameOptions.CustomModColors ? Colors.Dwarf : Colors.Modifier;
    public override string Name => !Smol && !Sped ? "Useless" : (!Smol ? "Flash" : (Sped ? "Gremlin" : "Dwarf"));
    public override LayerEnum Type => LayerEnum.Dwarf;
    public override Func<string> Description => () => !Smol && !Sped ? "- Why" : $"- You are {Text}";

    public Dwarf(PlayerControl player) : base(player) {}
}