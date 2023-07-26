namespace TownOfUsReworked.PlayerLayers.Modifiers
{
    public class Shy : Modifier
    {
        public override Color32 Color => ClientGameOptions.CustomModColors ? Colors.Shy : Colors.Modifier;
        public override string Name => "Shy";
        public override LayerEnum Type => LayerEnum.Shy;
        public override ModifierEnum ModifierType => ModifierEnum.Shy;
        public override Func<string> TaskText => () => "- You cannot call meetings";

        public Shy(PlayerControl player) : base(player) {}
    }
}