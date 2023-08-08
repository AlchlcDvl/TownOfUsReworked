namespace TownOfUsReworked.PlayerLayers.Modifiers;

public class Giant : Modifier
{
    private static bool Chonk => CustomGameOptions.GiantScale != 1;
    private static bool Snail => CustomGameOptions.GiantSpeed != 1;
    private static string Text => Chonk && Snail ? "big and slow" : (Chonk ? "big" : (Snail ? "slow" : ""));

    public override Color32 Color => ClientGameOptions.CustomModColors ? Colors.Giant : Colors.Modifier;
    public override string Name => !Chonk && !Snail ? "Useless" : (!Chonk ? "Sloth" : (Snail ? "Chonker" : "Giant"));
    public override LayerEnum Type => LayerEnum.Giant;
    public override Func<string> Description => () => !Chonk && !Snail ? "- Why" : $"- You are {Text}";

    public Giant(PlayerControl player) : base(player) {}
}