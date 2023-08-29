namespace TownOfUsReworked.PlayerLayers.Modifiers;

public class Giant : Modifier
{
    private static bool Chonk => CustomGameOptions.GiantScale != 1;
    private static bool Snail => CustomGameOptions.GiantSpeed != 1;
    private static bool Useless => !Chonk && !Snail;
    private static string Text => Chonk && Snail ? "big and slow" : (Chonk ? "big" : (Snail ? "slow" : ""));

    public override Color32 Color => ClientGameOptions.CustomModColors ? (Useless ? Colors.Modifier : Colors.Giant) : Colors.Modifier;
    public override string Name => Useless ? "Useless" : (!Chonk ? "Sloth" : (Snail ? "Chonker" : "Giant"));
    public override LayerEnum Type => LayerEnum.Giant;
    public override Func<string> Description => () => Useless ? "- Why" : $"- You are {Text}";

    public Giant(PlayerControl player) : base(player) {}
}