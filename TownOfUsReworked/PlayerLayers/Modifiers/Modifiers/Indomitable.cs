namespace TownOfUsReworked.PlayerLayers.Modifiers
{
    public class Indomitable : Modifier
    {
        public bool AttemptedGuess;

        public override Color32 Color => ClientGameOptions.CustomModColors ? Colors.Indomitable : Colors.Modifier;
        public override string Name => "Indomitable";
        public override LayerEnum Type => LayerEnum.Indomitable;
        public override ModifierEnum ModifierType => ModifierEnum.Indomitable;
        public override Func<string> TaskText => () => "- You cannot be guessed";
        public override bool Hidden => !CustomGameOptions.IndomitableKnows && !AttemptedGuess && !IsDead;

        public Indomitable(PlayerControl player) : base(player) => AttemptedGuess = false;
    }
}