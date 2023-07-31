namespace TownOfUsReworked.PlayerLayers.Modifiers
{
    public class Diseased : Modifier
    {
        public override Color32 Color => ClientGameOptions.CustomModColors ? Colors.Diseased : Colors.Modifier;
        public override string Name => "Diseased";
        public override LayerEnum Type => LayerEnum.Diseased;
        public override ModifierEnum ModifierType => ModifierEnum.Diseased;
        public override Func<string> TaskText => () => $"- Your killer's cooldown increases by {CustomGameOptions.DiseasedMultiplier} times";
        public override bool Hidden => !CustomGameOptions.DiseasedKnows && !IsDead;

        public Diseased(PlayerControl player) : base(player) {}
    }
}