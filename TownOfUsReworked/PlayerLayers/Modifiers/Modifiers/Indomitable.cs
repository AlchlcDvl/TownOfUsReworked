namespace TownOfUsReworked.PlayerLayers.Modifiers;

public class Indomitable : Modifier
{
    public bool AttemptedGuess { get; set; }

    public override Color32 Color => ClientGameOptions.CustomModColors ? Colors.Indomitable : Colors.Modifier;
    public override string Name => "Indomitable";
    public override LayerEnum Type => LayerEnum.Indomitable;
    public override Func<string> Description => () => "- You cannot be guessed";
    public override bool Hidden => !CustomGameOptions.IndomitableKnows && !AttemptedGuess && !IsDead;

    public Indomitable(PlayerControl player) : base(player) => AttemptedGuess = false;
}